using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarSystemAsLightPointViewComponent : BillboardPrimitive<StarSystemAsLightPointViewModel3D, StarSystemAsLightPointController>
    {
        public StarSystemAsLightPointViewComponent([NotNull] StarSystemAsLightPointViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
