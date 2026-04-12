using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Generation;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxySectorCloudView : RenderablePrimitive<GalaxySectorCloudViewModel3D, GalaxySectorCloudController>
    {
        private readonly ITextureGeneratorService _textureGeneratorService;

        public GalaxySectorCloudView(
            [NotNull] GalaxySectorCloudViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider,
            [NotNull] ITextureGeneratorService textureGeneratorService)
            : base(new FixedSimpleMesh(CreateGeometry(viewModel, camera3DProvider)), viewModel)
        {
            _textureGeneratorService = textureGeneratorService;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var texture = GaussianSpriteTexture.GetOrCreate(_textureGeneratorService);
            var material = new TextureMaterial(texture);
            Mesh.Material = material;
            DataModel.MeshMaterial = material;

            AddChild(new GalaxySectorSlabViewModel3D(
                DataModel.AggregatedData,
                DataModel.GalaxyAggregatedData,
                DataModel.Position));
        }

        private static SectorCloudGeometry CreateGeometry(
            GalaxySectorCloudViewModel3D viewModel, ICamera3DProvider camera3DProvider)
        {
            var agg = viewModel.AggregatedData;
            var cfg = GalaxyConfig.Instance.GalaxySectorTexture.Node;

            var galaxyRotation = Matrix.CreateRotationX(agg.Inclination)
                               * Matrix.CreateRotationY(agg.SpinAngle);

            return new SectorCloudGeometry(
                agg.ClusterPoints,
                galaxyRotation,
                () => camera3DProvider.GetActiveCamera().GetPosition(),
                viewModel.Position,
                agg.GalaxyColor,
                agg.SectorRadius,
                cfg.CloudSpriteRadius,
                cfg.CloudSpriteBrightness,
                cfg.CloudYScale,
                cfg.CloudGalaxyTintBlend);
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
