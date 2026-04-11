using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorTextureViewModel3D : ViewModel3D
    {
        public GalaxySectorTextureAggregatedData AggregatedData { get; }

        public GalaxySectorTextureViewModel3D(GalaxySectorTexture model)
        {
            AggregatedData = model.AggregatedData;
            Position = model.GetWorldPosition();
            Scale = new Vector3(model.AggregatedData.SectorRadius * 2f);
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
