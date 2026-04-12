using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Generation;

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
            var cfg = GalaxyConfig.Instance.GalaxySector.Node;
            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);
            return new SectorStarFieldGeometry(
                agg.ClusterPoints, galaxyRotation,
                () => camera3DProvider.GetActiveCamera().GetPosition(),
                viewModel.Position,
                cfg.DotBaseRadius, cfg.DotRadiusScale,
                cfg.DotBaseBrightness, cfg.DotBrightnessScale, cfg.DotEdgeBrightness);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var r = DataModel.AggregatedData.SectorRadius;
            var extent = new Vector3(r, r, r);
            return new BoundingBox(pos - extent, pos + extent);
        }
    }
}
