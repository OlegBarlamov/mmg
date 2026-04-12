using System;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Generation;

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

        private static SectorStarFieldGeometry CreateGeometry(
            GalaxySectorChunkViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cfg = GalaxyConfig.Instance.GalaxySectorChunk.Node;
            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);
            var brightnessMultiplier = (float)Math.Sqrt(cfg.DensityDampingReference / Math.Max(1, agg.ClusterPoints.Length));
            return new SectorStarFieldGeometry(
                agg.ClusterPoints, galaxyRotation,
                () => camera3DProvider.GetActiveCamera().GetPosition(),
                viewModel.Position,
                cfg.DotBaseRadius, cfg.DotRadiusScale,
                cfg.DotBaseBrightness, cfg.DotBrightnessScale, cfg.DotEdgeBrightness,
                brightnessMultiplier);
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
