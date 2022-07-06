using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using NetExtensions;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    [UsedImplicitly]
    internal class ResourcesService : IResourcesService, IDisposable
    {
        [NotNull] private IGameHeartServices GameHeartServices { get; }
        [NotNull] private IContentContainersFactory ContentContainersFactory { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorInternal { get; }
        [NotNull] private IRenderTargetsFactory RenderTargetsFactory { get; }

        [NotNull] private ModuleLogger Logger { get; }

        private bool _gameHeartResourcesLoaded;
        private bool _gameHeartResourcesUnloaded;

        private readonly ConcurrentDictionary<IResourcePackage, IContentContainer> _loadedPackages =
            new ConcurrentDictionary<IResourcePackage, IContentContainer>();
        
        private readonly List<LoadResourcePackageTask> _delayedPackagesForLoad = new List<LoadResourcePackageTask>();

        public ResourcesService(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IContentContainersFactory contentContainersFactory,
            [NotNull] ITextureGeneratorService textureGeneratorInternal,
            [NotNull] IRenderTargetsFactory renderTargetsFactory,
            [NotNull] IFrameworkLogger frameworkLogger)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            ContentContainersFactory = contentContainersFactory ?? throw new ArgumentNullException(nameof(contentContainersFactory));
            TextureGeneratorInternal = textureGeneratorInternal ?? throw new ArgumentNullException(nameof(textureGeneratorInternal));
            RenderTargetsFactory = renderTargetsFactory ?? throw new ArgumentNullException(nameof(renderTargetsFactory));
            Logger = new ModuleLogger(frameworkLogger, LogCategories.Resources);

            GameHeartServices.ResourceLoading += GameHeartServicesOnResourceLoading;
            GameHeartServices.ResourceUnloading += GameHeartServicesOnResourceUnloading;
        }
        
        public void Dispose()
        {
            GameHeartServices.ResourceLoading -= GameHeartServicesOnResourceLoading;
            GameHeartServices.ResourceUnloading -= GameHeartServicesOnResourceUnloading;
            
            _delayedPackagesForLoad.Clear();
        }

        public void LoadPackage([NotNull] IResourcePackage package, bool useBackgroundThread = true)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            var loadPackageTask = new LoadResourcePackageTask(package, useBackgroundThread);
            if (!_gameHeartResourcesLoaded)
            {
                _delayedPackagesForLoad.Add(loadPackageTask);
                return;
            }
            
            LoadPackage(loadPackageTask);
        }

        public void UnloadPackage([NotNull] IResourcePackage package, bool useBackgroundThread = true)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            
            var unloadPackageTask = new LoadResourcePackageTask(package, useBackgroundThread);
            if (_delayedPackagesForLoad.Contains(unloadPackageTask))
            {
                _delayedPackagesForLoad.Remove(unloadPackageTask);
                return;
            }
            
            UnloadPackage(unloadPackageTask);
        }

        private void LoadPackage(LoadResourcePackageTask loadResourcePackageTask)
        {
            var package = loadResourcePackageTask.Package;
            Logger.Info(Strings.Info.Resources.StartLoadingResourcePackage, package.GetTypeName());

            try
            {
                var contentContainer = ContentContainersFactory.Create(package);
                if (_loadedPackages.TryAdd(package, contentContainer))
                {
                    var apiContainer = new ContentLoaderApi(contentContainer, TextureGeneratorInternal, RenderTargetsFactory);
                    package.Load(apiContainer);
                    Logger.Info(Strings.Info.Resources.FinishLoadingResourcePackage, package.GetTypeName());
                }
                else
                {
                    contentContainer.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Error(Strings.Exceptions.Resources.LoadResourceFailed, e, package.GetTypeName());
            }
        }

        private void UnloadPackage(LoadResourcePackageTask loadResourcePackageTask)
        {
            var package = loadResourcePackageTask.Package;
            Logger.Info(Strings.Info.Resources.StartUnloadingResourcePackage, package.GetTypeName());
            
            try
            {
                if (!_loadedPackages.TryRemove(package, out var container))
                    return;

                using (container)
                {
                    container.Unload();
                    package.OnUnloaded();
                }
                
                Logger.Info(Strings.Info.Resources.FinishedUnloadingResourcePackage, package.GetTypeName());
            }
            catch (Exception e)
            {
                Logger.Error(Strings.Exceptions.Resources.UnloadResourceFailed, e, package.GetTypeName());
            }
        }

        private void GameHeartServicesOnResourceLoading()
        {
            GameHeartServices.ResourceLoading -= GameHeartServicesOnResourceLoading;
            _gameHeartResourcesLoaded = true;

            while (_delayedPackagesForLoad.Count > 0)
            {
                var package = _delayedPackagesForLoad[0];
                _delayedPackagesForLoad.Remove(package);
                LoadPackage(package);
            }
        }

        private void GameHeartServicesOnResourceUnloading()
        {
            GameHeartServices.ResourceUnloading -= GameHeartServicesOnResourceUnloading;
            _gameHeartResourcesUnloaded = true;
        }
    }
}