namespace FrameworkSDK.MonoGame.Resources
{
    public interface IResourcesService
    {
        void LoadPackage(IResourcePackage package, bool useBackgroundThread = true);

        void UnloadPackage(IResourcePackage package, bool useBackgroundThread = true);
    }
}