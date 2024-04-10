using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
    public interface ILocatable2D : IPositioned2D
    {
        void SetPosition(Vector2 position);
    }
}