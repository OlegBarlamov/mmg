using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace FrameworkSDK.MonoGame.Map
{
    public class Tiled2DCell : IGridCell<Point>
    {
        public Point MapPoint { get; }
        
        public IDrawableStencil Visual { get; }

        public Tiled2DCell(Point mapPoint, [NotNull] IDrawableStencil visual)
        {
            MapPoint = mapPoint;
            Visual = visual ?? throw new ArgumentNullException(nameof(visual));
        }
        
        public Point GetPointOnMap()
        {
            return MapPoint;
        }
    }
}