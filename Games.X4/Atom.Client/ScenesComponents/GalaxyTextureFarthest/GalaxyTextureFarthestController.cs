using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class GalaxyTextureFarthestController : BillboardController<GalaxyTextureFarthestViewModel3D>
    {
        public GalaxyTextureFarthestController(
            [NotNull] GalaxyTextureFarthestViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(viewModel, camera3DProvider)
        {
        }
    }
}
