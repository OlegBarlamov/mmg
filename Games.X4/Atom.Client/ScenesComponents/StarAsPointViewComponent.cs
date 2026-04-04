using Atom.Client.ViewModels;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarAsPointViewComponent : BillboardPrimitive<StarViewModel3D, StarAsPointController>
    {
        public StarAsPointViewComponent([NotNull] StarViewModel3D viewModel)
            : base(viewModel, viewModel.GraphicsPassName)
        {
        }
    }
}
