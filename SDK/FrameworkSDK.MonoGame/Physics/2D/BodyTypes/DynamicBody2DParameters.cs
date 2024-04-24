using FrameworkSDK.MonoGame.Physics2D;

namespace FrameworkSDK.MonoGame.Physics._2D.BodyTypes
{
    public class DynamicBody2DParameters : IPhysicsBody2DParameters
    {
        public bool ForcesTarget { get; } = true;
        public bool Static { get; } = false;
        public float Mass { get; set; } = 1f;
        public float FrictionFactor { get; } = 0.1f;
        public float AngularFrictionFactor { get; } = 0.1f;
        public float BounceFactor { get; } = 0f;
    }
}