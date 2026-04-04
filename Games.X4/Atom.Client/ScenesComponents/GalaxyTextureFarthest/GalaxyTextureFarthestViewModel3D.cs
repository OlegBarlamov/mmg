using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxyTextureFarthestViewModel3D : ViewModel3D
    {
        public GalaxyTextureFarthestViewModel3D(GalaxyTextureFarthest model, Texture2D galaxyTexture)
        {
            Position = model.GetWorldPosition();
            Scale = new Vector3(model.AggregatedData.GalaxyData.Power * 50f);
            MeshMaterial = new TextureMaterial(galaxyTexture);
            GraphicsPassName = "Render_Textured_No_Lights";
        }
    }
}
