using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Basic
{
    public interface IScenePlaceable : IPlaceable3D
    {
        Matrix World { get; }
    }
}