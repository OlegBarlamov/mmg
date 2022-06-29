using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.Basic
{
    public interface IPlaceable3D : ILocatable3D
    {
        Vector3 Scale { get; set; }
        
        Vector3 Rotation { get; set; }
    }
}