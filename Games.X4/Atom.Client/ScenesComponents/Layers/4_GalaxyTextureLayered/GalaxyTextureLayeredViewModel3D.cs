using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxyTextureLayeredViewModel3D : ViewModel3D
    {
        public GalaxyTextureLayeredAggregatedData AggregatedData { get; }

        public GalaxyTextureLayeredViewModel3D(GalaxyTextureLayered model)
        {
            AggregatedData = model.AggregatedData;
            Position = model.GetWorldPosition();
            Scale = new Vector3(model.AggregatedData.DiskRadius * 2f);
            Rotation = Matrix.CreateRotationX(model.AggregatedData.Inclination)
                     * Matrix.CreateRotationY(model.AggregatedData.SpinAngle);
            GraphicsPassName = GraphicsPasses.TexturedNoLights;
        }

        public override void Dispose()
        {
            AggregatedData.TextureData.Dispose();
            base.Dispose();
        }
    }
}
