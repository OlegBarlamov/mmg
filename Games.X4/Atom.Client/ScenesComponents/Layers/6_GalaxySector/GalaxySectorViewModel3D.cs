using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorViewModel3D : ViewModel3D
    {
        public GalaxySectorAggregatedData AggregatedData { get; }
        public GalaxySectorTextureAggregatedData SectorTextureAggregatedData { get; }
        public GalaxyTextureLayeredAggregatedData GalaxyAggregatedData { get; }

        public GalaxySectorViewModel3D(GalaxySector sector)
        {
            AggregatedData = sector.AggregatedData;
            SectorTextureAggregatedData = (GalaxySectorTextureAggregatedData)sector.Parent.AggregatedData;
            GalaxyAggregatedData = (GalaxyTextureLayeredAggregatedData)sector.Parent.Parent.AggregatedData;
            Position = sector.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
