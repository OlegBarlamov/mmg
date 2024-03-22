using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;

namespace Template.MacOs
{
    public class OmegasGameApp : GameApp
    {
        protected override SceneBase CurrentScene { get; } = new EmptyScene();
    }
}