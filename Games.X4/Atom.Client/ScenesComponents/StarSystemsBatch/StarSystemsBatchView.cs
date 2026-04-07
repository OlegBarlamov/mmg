using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarSystemsBatchView : RenderablePrimitive<StarSystemsBatchViewModel3D>
    {
        public StarSystemsBatchView(
            [NotNull] StarSystemsBatchViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static StarSystemsBatchGeometry CreateGeometry(
            StarSystemsBatchViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cameraPos = camera3DProvider.GetActiveCamera().GetPosition();
            var localCameraDir = cameraPos - viewModel.Position;
            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);
            return new StarSystemsBatchGeometry(agg.ClusterPoints, agg.BatchRadius, localCameraDir, galaxyRotation);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var r = DataModel.AggregatedData.BatchRadius;
            var extent = new Vector3(r, r, r);
            return new BoundingBox(pos - extent, pos + extent);
        }
    }
}
