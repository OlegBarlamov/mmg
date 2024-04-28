using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.Map
{
    public class Tiled2DMap : Array2DBasedGrid<Tiled2DCell> 
    {
        public Vector2 CellsSize { get; }

        public Vector2 WorldSize { get; }

        public Tiled2DMap([NotNull] Tiled2DCell[,] data, Vector2 cellsSize) : base(data)
        {
            CellsSize = cellsSize;
            WorldSize = new Vector2(Width * cellsSize.X, Height * cellsSize.Y);
        }

        public Point GetMapPointFromWorld(Vector2 world)
        {
            return (world / CellsSize).ToPoint();
        }
    }
}