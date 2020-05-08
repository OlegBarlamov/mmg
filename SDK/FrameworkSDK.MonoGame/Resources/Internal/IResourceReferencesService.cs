using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal interface IResourceReferencesService : IDisposable
    {
        void AddResourceReference(ResourceRecord resourceRecord, IContentContainer owner);

        void RemoveResourceReference(ResourceRecord resourceRecord, IContentContainer owner);

        IEnumerable<IContentContainer> FindResourceOwners(ResourceRecord resourceRecord);

        int ResourceReferencesCount(ResourceRecord resourceRecord);

        void AddPackageless(IDisposable resource);
        void RemovePackageless(IDisposable resource);
    }
}