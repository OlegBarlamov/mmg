using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemAsPointViewModel3D : ViewModel3D
    {
        public StarSystemAsPointViewModel3D(StarSystemAsPoint starSystem)
        {
            Position = starSystem.GetWorldPosition();
            Scale = new Vector3(starSystem.AggregatedData.Luminosity * 1.5f);
            MeshMaterial = new StarMaterial(starSystem.AggregatedData.Color, MathHelper.Clamp(starSystem.AggregatedData.Luminosity * 0.8f, 0.3f, 1f));
            GraphicsPassName = GraphicsPasses.Stars;
        }
    }
}
