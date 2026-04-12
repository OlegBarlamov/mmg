using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemAsPointViewModel3D : ViewModel3D
    {
        public StarSystemAsPointAggregatedData AggregatedData { get; }

        public StarSystemAsPointViewModel3D(StarSystemAsPoint starSystem)
        {
            AggregatedData = starSystem.AggregatedData;
            Position = starSystem.GetWorldPosition();
            Scale = Vector3.One;
            GraphicsPassName = GraphicsPasses.ColoredStars;
        }
    }
}
