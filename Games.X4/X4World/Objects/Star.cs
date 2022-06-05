using Microsoft.Xna.Framework;

namespace X4World.Objects
{
    public class Star
    {
        public Vector3 WorldPosition { get; }

        public Vector3 Size { get; } = new Vector3(0.1f);
        
        public Galaxy Owner { get; }

        public Star(Vector3 worldPosition, Galaxy owner)
        {
            Owner = owner;
            WorldPosition = worldPosition;
        }
    }
}