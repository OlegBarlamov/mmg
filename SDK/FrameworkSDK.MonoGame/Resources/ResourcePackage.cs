using System;
using FrameworkSDK.Common;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Localization;

namespace FrameworkSDK.MonoGame.Resources
{
    public abstract class ResourcePackage : IResourcePackage
    {
        public event Action Loaded;
        
        public bool IsLoaded { get; private set; }
        public bool IsLoading { get; private set; }

        private static Lazy<ModuleLogger> LazyLoggerFactory { get; } = 
            new Lazy<ModuleLogger>(() => new ModuleLogger(AppContext.Logger, FrameworkLogModule.Resources));
        private static ModuleLogger Logger => LazyLoggerFactory.Value;
        
        private bool _unloaded;
        private bool _isDisposed;
        
        private readonly object _loadLocker = new object();

        protected abstract void Load(IContentLoaderApi content);

        public virtual void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ResourcePackage));
            if (!_unloaded)
                Logger.Warn(Strings.Warnings.Resources.ResourcePackageNotUnloaded, this.GetTypeName());

            _isDisposed = true;
            Loaded = null;
            Logger.Dispose();
        }

        void IResourcePackage.OnUnloaded()
        {
            if (!IsLoaded)
                return;

            lock (_loadLocker)
            {
                if (!IsLoaded)
                    return;
                
                IsLoaded = false;
                _unloaded = true;
            }
        }
        
        void IResourcePackage.Load(IContentLoaderApi content)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_isDisposed) throw new ObjectDisposedException(nameof(ResourcePackage));
            
            if (IsLoaded || IsLoading)
                return;
            
            lock (_loadLocker)
            {
                if (IsLoaded || IsLoading || _isDisposed)
                    return;

                IsLoading = true;
                _unloaded = false;
                try
                {
                    Load(content);
                }
                finally
                {
                    IsLoading = false;
                }

                IsLoaded = true;
            }

            Loaded?.Invoke();
        }
    }
}