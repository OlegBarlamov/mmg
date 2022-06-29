using FrameworkSDK.MonoGame.Graphics.Materials;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models
{
    public abstract class SolidPrimitiveViewModel : ViewModel3D
    {
        protected SolidPrimitiveViewModel()
        {
            MeshMaterial = StaticMaterials.DefaultSolidMaterial;
        }
    }
}