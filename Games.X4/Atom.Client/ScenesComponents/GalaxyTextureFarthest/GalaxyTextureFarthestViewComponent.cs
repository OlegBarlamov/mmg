using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxyTextureFarthestViewComponent : BillboardPrimitive<GalaxyTextureFarthestViewModel3D, GalaxyTextureFarthestController>
    {
        public GalaxyTextureFarthestViewComponent([NotNull] GalaxyTextureFarthestViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
