namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IPhysicsBody2DParameters
    {
        bool ForcesTarget { get; }
        bool Static { get; }

        float Mass { get; }
        float FrictionFactor { get; }
        
        float AngularFrictionFactor { get; }
        
        float BounceFactor { get; }
    }
}