using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.ViewModels
{
    public class StarViewModel3D : ViewModel3D
    {
        public StarViewModel3D(StarAsPoint star)
        {
            Position = star.GetWorldPosition();
            Scale = new Vector3(star.AggregatedData.Power * 40);
            MeshMaterial = new StarMaterial(star.AggregatedData.Color, star.AggregatedData.Power);
            GraphicsPassName = "Render_Stars";
        }
    }
}
