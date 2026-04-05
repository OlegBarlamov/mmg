using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorViewModel3D : ViewModel3D
    {
        public GalaxySectorAggregatedData AggregatedData { get; }

        public GalaxySectorViewModel3D(GalaxySector sector)
        {
            AggregatedData = sector.AggregatedData;
            Position = sector.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
