using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class GalaxyAsPointController : BillboardController<GalaxyAsPointViewModel3D>
    {
        public GalaxyAsPointController(
            [NotNull] GalaxyAsPointViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(viewModel, camera3DProvider)
        {
        }
    }
}
