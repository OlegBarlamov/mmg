namespace FrameworkSDK.MonoGame.Resources
{
    public interface IContentLoader
    {
        T Load<T>(string assetName);
    }
}