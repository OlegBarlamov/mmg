using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.Graphics.Materials
{
    public static class StaticMaterials
    {
        public static IMeshMaterial EmptyMaterial { get; } = new EmptyMaterial();

        public static IMeshMaterial DefaultSolidMaterial { get; private set; } = EmptyMaterial; 
        
        internal static void InitializeDefaultMaterial(GraphicsDevice graphicsDevice)
        {
            DefaultSolidMaterial = new TextureMaterial(graphicsDevice.GetTextureDiffuseColor(Color.White));
        }
    }
}