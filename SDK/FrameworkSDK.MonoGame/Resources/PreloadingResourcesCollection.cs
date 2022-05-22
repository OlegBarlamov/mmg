using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Resources
{
    internal class PreloadingResourcesCollection
    {
        public IReadOnlyCollection<IResourcePackage> Resources => _resources;

        private readonly List<IResourcePackage> _resources;

        public PreloadingResourcesCollection([NotNull] params IResourcePackage[] resources)
        {
            if (resources == null) throw new ArgumentNullException(nameof(resources));
            
            _resources = new List<IResourcePackage>(resources);
        }

        public void Add([NotNull] IResourcePackage package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            
            _resources.Add(package);
        }

        public void Clear()
        {
            _resources.Clear();
        }
    }
}