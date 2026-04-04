using Atom.Client.ViewModels;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class SphereViewComponent : BillboardPrimitive<PlanetSystemViewModel3D, SphereController>
    {
        public SphereViewComponent([NotNull] PlanetSystemViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
