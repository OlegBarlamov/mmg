using FrameworkSDK;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace X4World.Objects
{
    public class Star: ILocatable3D, INamed
    {
        public Vector3 WorldPosition => GetWorldPosition(); 
        
        public Vector3 Position { get; }
        public string Name { get; }
        
        public Vector3 Size { get; } = new Vector3(1f);
        
        public Galaxy Owner { get; }

        public Star(Vector3 localPosition, Galaxy owner, string name)
        {
            Position = localPosition;
            Owner = owner;
            Name = name;
        }

        private Vector3 GetWorldPosition()
        {
            return Owner.Position + Position;
        }
    }
}