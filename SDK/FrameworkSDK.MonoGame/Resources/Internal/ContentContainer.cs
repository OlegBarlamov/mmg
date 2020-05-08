using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Content;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal class ContentContainer : IContentContainer
    {
        public IResourcePackage Package { get; }
        
        private IResourceReferencesService ResourceReferencesService { get; }

        private static ModuleLogger Logger { get; } = new ModuleLogger(AppContext.Logger, FrameworkLogModule.Resources); 
        
        private bool _isDisposed;
        
        private readonly ContentManager _monoContentManager; 
        private readonly List<ResourceRecord> _loadedResources = new List<ResourceRecord>();
        private readonly List<IDisposable> _loadedUnmanaged = new List<IDisposable>();

        public ContentContainer(
            [NotNull] IGameParameters gameParameters,
            [NotNull] IGameHeartServices heartServices,
            [NotNull] IResourceReferencesService resourceReferencesService,
            [NotNull] IResourcePackage package)
        {
            if (gameParameters == null) throw new ArgumentNullException(nameof(gameParameters));
            if (heartServices == null) throw new ArgumentNullException(nameof(heartServices));
            ResourceReferencesService = resourceReferencesService ?? throw new ArgumentNullException(nameof(resourceReferencesService));
            Package = package ?? throw new ArgumentNullException(nameof(package));

            _monoContentManager = new ContentManager(
                heartServices.MonoGameServiceContainer,
                gameParameters.ContentRootDirectory);
        }

        public T Load<T>(string assetName)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ContentContainer));
            
            var result = _monoContentManager.Load<T>(assetName);
            
            var record = ResourceRecord.Create<T>(assetName);
            _loadedResources.Add(record);
            ResourceReferencesService.AddResourceReference(record, this);
            
            return result;
        }

        public void AddResource([NotNull] IDisposable disposableResource)
        {
            if (disposableResource == null) throw new ArgumentNullException(nameof(disposableResource));
            if (_isDisposed) throw new ObjectDisposedException(nameof(ContentContainer));
            
            _loadedUnmanaged.Add(disposableResource);
        }

        public void Unload()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ContentContainer));
            
            var resourceWithMultipleOwners = FindResourcesHaveMultipleOwners(_loadedResources);
            if (resourceWithMultipleOwners.Count > 0)
            {
                Logger.Error(Strings.Errors.Resources.ResourceAssetsReferencedByAnotherPackage, Package.GetTypeName(), resourceWithMultipleOwners.Count);
                Logger.LogCollection(resourceWithMultipleOwners, record => $"  {record}", FrameworkLogLevel.Error);
                return;
            }

            foreach (var loadedResource in _loadedResources)
            {
                ResourceReferencesService.RemoveResourceReference(loadedResource, this);   
            }
            
            _monoContentManager.Unload();
            _loadedResources.Clear();

            foreach (var unmanaged in _loadedUnmanaged)
            {
                using (unmanaged)
                {
                    //dispose
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ContentContainer));

            _isDisposed = true;
            _monoContentManager.Dispose();
        }

        private IReadOnlyList<ResourceRecord> FindResourcesHaveMultipleOwners(IEnumerable<ResourceRecord> resources)
        {
            return resources.Where(x => ResourceReferencesService.ResourceReferencesCount(x) > 1).ToArray();
        }
    }
}