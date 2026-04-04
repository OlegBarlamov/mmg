using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.ViewModels
{
    public class PlanetSystemViewModel3D : ViewModel3D
    {
        public PlanetSystemViewModel3D(PlanetSystemFarthest model, Texture2D galaxyTexture)
        {
            Position = model.GetWorldPosition();
            Scale = new Vector3(model.AggregatedData.StarData.Power * 50f);
            MeshMaterial = new TextureMaterial(galaxyTexture);
            GraphicsPassName = "Render_Textured_No_Lights";
        }
    }
}
