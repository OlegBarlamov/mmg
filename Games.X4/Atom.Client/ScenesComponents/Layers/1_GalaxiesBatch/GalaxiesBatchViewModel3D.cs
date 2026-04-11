using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxiesBatchViewModel3D : ViewModel3D
    {
        public GalaxiesBatchAggregatedData AggregatedData { get; }

        public GalaxiesBatchViewModel3D(GalaxiesBatch batch)
        {
            AggregatedData = batch.AggregatedData;
            Position = batch.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
