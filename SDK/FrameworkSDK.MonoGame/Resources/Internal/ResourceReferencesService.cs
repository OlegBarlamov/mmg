using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using NetExtensions;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    [UsedImplicitly]
    internal class ResourceReferencesService : IResourceReferencesService
    {
        private readonly ConcurrentDictionary<ResourceRecord, RefToValue<int>> _resourcesRefCounters = 
            new ConcurrentDictionary<ResourceRecord, RefToValue<int>>();
        private readonly ConcurrentDictionary<ResourceRecord, List<IContentContainer>> _resourcesOwners =
            new ConcurrentDictionary<ResourceRecord, List<IContentContainer>>();
        private readonly ConcurrentDictionary<Type, RefToValue<int>> _packagelessResourcesRefCounters
            = new ConcurrentDictionary<Type, RefToValue<int>>();
        
        public void Dispose()
        {
            //TODO Check if all resources clear!
        }
        
        public void AddResourceReference([NotNull] ResourceRecord resourceRecord, [NotNull] IContentContainer owner)
        {
            if (resourceRecord == null) throw new ArgumentNullException(nameof(resourceRecord));
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            var counter = _resourcesRefCounters.GetOrAdd(resourceRecord, new RefToValue<int>(0));
            counter.Value++;

            var owners = _resourcesOwners.GetOrAdd(resourceRecord, new List<IContentContainer>());
            owners.Add(owner);
        }

        public void RemoveResourceReference([NotNull] ResourceRecord resourceRecord, [NotNull] IContentContainer owner)
        {
            if (resourceRecord == null) throw new ArgumentNullException(nameof(resourceRecord));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            
            var counter = _resourcesRefCounters.GetOrAdd(resourceRecord, new RefToValue<int>(0));
            counter.Value--;
            
            var owners = _resourcesOwners.GetOrAdd(resourceRecord, new List<IContentContainer>());
            owners.Remove(owner);
        }

        public IEnumerable<IContentContainer> FindResourceOwners(ResourceRecord resourceRecord)
        {
            throw new System.NotImplementedException();
        }

        public int ResourceReferencesCount(ResourceRecord resourceRecord)
        {
            return _resourcesRefCounters[resourceRecord].Value;
        }

        public void AddPackageless([NotNull] IDisposable resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var type = resource.GetType();
            var counter = _packagelessResourcesRefCounters.GetOrAdd(type, new RefToValue<int>());
            counter.Value++;
        }

        public void RemovePackageless([NotNull] IDisposable resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            var type = resource.GetType();
            var counter = _packagelessResourcesRefCounters.GetOrAdd(type, new RefToValue<int>());
            counter.Value--;
        }
    }
}