using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IFixture2D
    {
        IColliderBody2D Parent { get; }
        Vector2 Center { get; }
        bool Contains(Vector2 point);
    }
}