using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class SectorDimStarFieldView : RenderablePrimitive<SectorDimStarFieldViewModel3D>
    {
        public SectorDimStarFieldView(
            [NotNull] SectorDimStarFieldViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static SectorDimStarFieldGeometry CreateGeometry(
            SectorDimStarFieldViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cameraPos = camera3DProvider.GetActiveCamera().GetPosition();
            var localCameraDir = cameraPos - viewModel.Position;
            return new SectorDimStarFieldGeometry(agg.SectorRadius, agg.StarCount, agg.Seed, localCameraDir);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return null;
        }
    }
}
