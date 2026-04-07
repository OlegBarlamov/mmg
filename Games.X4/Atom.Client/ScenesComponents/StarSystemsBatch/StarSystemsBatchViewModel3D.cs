using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemsBatchViewModel3D : ViewModel3D
    {
        public StarSystemsBatchAggregatedData AggregatedData { get; }

        public StarSystemsBatchViewModel3D(StarSystemsBatch batch)
        {
            AggregatedData = batch.AggregatedData;
            Position = batch.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
