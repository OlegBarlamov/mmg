using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorChunkViewModel3D : ViewModel3D
    {
        public GalaxySectorChunkAggregatedData AggregatedData { get; }

        public GalaxySectorChunkViewModel3D(GalaxySectorChunk chunk)
        {
            AggregatedData = chunk.AggregatedData;
            Position = chunk.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
