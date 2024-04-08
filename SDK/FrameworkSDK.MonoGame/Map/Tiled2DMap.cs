using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.Map
{
    public class Tiled2DMap : Array2DBasedGrid<Tiled2DCell> 
    {
        public Vector2 CellsSize { get; }
        
        public Tiled2DMap([NotNull] Tiled2DCell[,] data, Vector2 cellsSize) : base(data)
        {
            CellsSize = cellsSize;
        }

        public Point GetMapPointFromWorld(Vector2 world)
        {
            return (world / CellsSize).ToPoint();
        }
    }
}