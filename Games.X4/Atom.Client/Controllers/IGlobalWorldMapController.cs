using System;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Controllers
{
    public interface IGlobalWorldMapController
    {
        event Action<WorldMapCellContent> CellRevealed;
        event Action<WorldMapCellContent> CellHidden;
        event Action<WorldMapCellContent> CellUnwrapped;
        event Action<WorldMapCellContent> CellWrapped;

        void Update(Vector3 playerPosition, GameTime gameTime);
    }
}