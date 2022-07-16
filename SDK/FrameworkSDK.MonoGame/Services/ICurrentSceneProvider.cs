using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Services
{
    public interface ICurrentSceneProvider
    {
        SceneBase CurrentScene { get; }
    }
}