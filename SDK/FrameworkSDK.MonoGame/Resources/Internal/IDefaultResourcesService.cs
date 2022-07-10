namespace FrameworkSDK.MonoGame.Resources
{
    public interface IDefaultResourcesService
    {
        bool HasDefaultVersionFor<T>();
        T GetDefaultVersionFor<T>();
    }
}