using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal interface IContentContainer : IContentLoader, IDisposable
    {
        IResourcePackage Package { get; }
        void AddResource(IDisposable disposableResource);
        void Unload();
    }
}