using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
    public interface ILocatable3D : IPositioned3D
    {
        void SetPosition(Vector3 position);
    }
}