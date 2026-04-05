using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class SectorDimStarFieldViewModel3D : ViewModel3D
    {
        public SectorDimStarFieldAggregatedData AggregatedData { get; }

        public SectorDimStarFieldViewModel3D(SectorDimStarField model)
        {
            AggregatedData = model.AggregatedData;
            Position = model.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
