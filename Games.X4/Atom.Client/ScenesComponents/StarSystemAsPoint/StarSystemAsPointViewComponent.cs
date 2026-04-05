using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarSystemAsPointViewComponent : BillboardPrimitive<StarSystemAsPointViewModel3D, StarSystemAsPointController>
    {
        public StarSystemAsPointViewComponent([NotNull] StarSystemAsPointViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
