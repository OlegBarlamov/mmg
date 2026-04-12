using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class GalaxySectorCloudController : Controller<GalaxySectorCloudViewModel3D>
    {
        public GalaxySectorCloudController(
            [NotNull] GalaxySectorCloudViewModel3D viewModel)
        {
            SetModel(viewModel);
        }
    }
}
