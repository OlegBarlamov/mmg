using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxySectorChunkView : RenderablePrimitive<GalaxySectorChunkViewModel3D>
    {
        public GalaxySectorChunkView(
            [NotNull] GalaxySectorChunkViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static GalaxySectorChunkGeometry CreateGeometry(
            GalaxySectorChunkViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);
            return new GalaxySectorChunkGeometry(
                agg.ClusterPoints, agg.ChunkRadius, galaxyRotation,
                () => camera3DProvider.GetActiveCamera().GetPosition(),
                viewModel.Position);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var r = DataModel.AggregatedData.ChunkRadius;
            var extent = new Vector3(r, r, r);
            return new BoundingBox(pos - extent, pos + extent);
        }
    }
}
