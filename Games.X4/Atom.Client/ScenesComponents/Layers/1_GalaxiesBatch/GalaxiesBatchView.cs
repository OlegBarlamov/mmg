using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxiesBatchView : RenderablePrimitive<GalaxiesBatchViewModel3D>
    {
        public GalaxiesBatchView(
            [NotNull] GalaxiesBatchViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static GalaxiesBatchGeometry CreateGeometry(
            GalaxiesBatchViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cameraPos = camera3DProvider.GetActiveCamera().GetPosition();
            var localCameraDir = cameraPos - viewModel.Position;
            return new GalaxiesBatchGeometry(agg.GalaxyPoints, localCameraDir);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var r = DataModel.AggregatedData.CellSize * 0.5f;
            var extent = new Vector3(r, r, r);
            return new BoundingBox(pos - extent, pos + extent);
        }
    }
}
