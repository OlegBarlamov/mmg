using Microsoft.Xna.Framework;
using MonoGameExtensions;
using X4World.Maps;

namespace X4World.Objects
{
    public interface IGalaxiesLevelObject : ILocatable3D
    {
        Vector3 Size { get; }

        GlobalWorldMapCell OwnedCell { get; }
    }
}