using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxyAsPointViewComponent : BillboardPrimitive<GalaxyAsPointViewModel3D, GalaxyAsPointController>
    {
        public GalaxyAsPointViewComponent([NotNull] GalaxyAsPointViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
