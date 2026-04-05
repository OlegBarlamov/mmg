using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxyAsPointViewModel3D : ViewModel3D
    {
        public GalaxyAsPointViewModel3D(GalaxyAsPoint galaxy)
        {
            Position = galaxy.GetWorldPosition();
            Scale = new Vector3(galaxy.AggregatedData.Power * 60);
            MeshMaterial = new StarMaterial(galaxy.AggregatedData.Color, galaxy.AggregatedData.Power);
            GraphicsPassName = GraphicsPasses.Stars;
        }
    }
}
