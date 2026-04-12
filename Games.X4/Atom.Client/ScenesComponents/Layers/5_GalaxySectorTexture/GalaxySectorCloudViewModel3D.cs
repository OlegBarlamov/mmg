using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorCloudViewModel3D : ViewModel3D
    {
        public GalaxySectorTextureAggregatedData AggregatedData { get; }
        public GalaxyTextureLayeredAggregatedData GalaxyAggregatedData { get; }

        public GalaxySectorCloudViewModel3D(GalaxySectorTexture model)
        {
            AggregatedData = model.AggregatedData;
            GalaxyAggregatedData = (GalaxyTextureLayeredAggregatedData)model.Parent.AggregatedData;
            Position = model.GetWorldPosition();
            Scale = Vector3.One;
            Rotation = Matrix.Identity;
            GraphicsPassName = GraphicsPasses.CloudSprites;
        }

        public override void Dispose()
        {
            AggregatedData.TextureData.Dispose();
            base.Dispose();
        }
    }
}
