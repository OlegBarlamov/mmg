using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.Map
{
    public class Tiled2DCell : IGridCell<Point>
    {
        public Point MapPoint { get; }
        
        public Texture2D Texture { get; }

        public Tiled2DCell(Point mapPoint, [NotNull] Texture2D texture)
        {
            MapPoint = mapPoint;
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }
        
        public Point GetPointOnMap()
        {
            return MapPoint;
        }
    }
    
    public class Tiled2DMap<TCell> : Array2DBasedGrid<TCell> where TCell : Tiled2DCell
    {
        public Tiled2DMap([NotNull] TCell[,] data) : base(data)
        {
        }
    }
}