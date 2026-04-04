using Atom.Client.ViewModels;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class SphereController : BillboardController<PlanetSystemViewModel3D>
    {
        public SphereController(
            [NotNull] PlanetSystemViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(viewModel, camera3DProvider)
        {
        }
    }
}
