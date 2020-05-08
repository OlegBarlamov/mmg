using FrameworkSDK.MonoGame.Resources.Generation;

namespace FrameworkSDK.MonoGame.Resources
{
    /// <summary>
    /// Generate resource and add it to container
    /// </summary>
    public interface IContentLoaderApi : IContentLoader, ITextureGeneratorApi, IRenderTargetsFactory
    {
    }
}