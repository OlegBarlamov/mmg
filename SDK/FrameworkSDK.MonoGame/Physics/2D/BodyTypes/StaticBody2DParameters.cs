using FrameworkSDK.MonoGame.Physics2D;

namespace FrameworkSDK.MonoGame.Physics._2D.BodyTypes
{
    public class StaticBody2DParameters : IPhysicsBody2DParameters
    {
        public bool ForcesTarget { get; } = false;
        public bool Static { get; } = true;
        public float Mass { get; } = float.MaxValue;
        public float FrictionFactor { get; } = float.MaxValue;
        public float AngularFrictionFactor { get; } = float.MaxValue;
        public float BounceFactor { get; } = 0;
    }
}