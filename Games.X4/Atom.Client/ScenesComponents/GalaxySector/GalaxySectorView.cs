using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxySectorView : RenderablePrimitive<GalaxySectorViewModel3D>
    {
        public GalaxySectorView(
            [NotNull] GalaxySectorViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
        }

        private static SectorStarFieldGeometry CreateGeometry(
            GalaxySectorViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cameraPos = camera3DProvider.GetActiveCamera().GetPosition();
            var localCameraDir = cameraPos - viewModel.Position;
            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);
            return new SectorStarFieldGeometry(agg.ClusterPoints, agg.SectorRadius, localCameraDir, galaxyRotation);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return null;
        }
    }
}
