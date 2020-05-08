// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal interface IContentContainersFactory
    {
        IContentContainer Create(IResourcePackage package);
    }
}