namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IColliderBody2D : IPhysicsBody2D
    {
        bool NoClipMode { get; }
        IFixture2D Fixture { get; }

        bool OnCollision(IColliderBody2D body);
    }
}