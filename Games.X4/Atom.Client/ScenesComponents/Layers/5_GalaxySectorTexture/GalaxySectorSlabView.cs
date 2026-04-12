using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Generation;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxySectorSlabView : RenderablePrimitive<GalaxySectorSlabViewModel3D, GalaxySectorSlabController>
    {
        private readonly ICamera3DProvider _camera3DProvider;
        private GalaxySlabMaterial _slabMaterial;

        public GalaxySectorSlabView(
            [NotNull] GalaxySectorSlabViewModel3D viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(StaticGeometries.Cube), viewModel)
        {
            _camera3DProvider = camera3DProvider;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var cfg = GalaxyConfig.Instance.GalaxySectorTexture.Node;
            _slabMaterial = new GalaxySlabMaterial(
                () => _camera3DProvider.GetActiveCamera().GetPosition(),
                cfg.SlabBrightness,
                cfg.SlabFalloffSharpness,
                DataModel.SectorUVOffset,
                DataModel.SectorUVScale,
                cfg.SlabEdgeFadeStart);

            var slabTextureData = DataModel.ParentAggregatedData.SlabTextureData;
            slabTextureData.TextureChanged += OnTextureChanged;
            DataModel.SlabTextureReady += OnSlabTextureReady;

            var existing = slabTextureData.Texture;
            if (existing != null)
                ApplyTexture(existing);
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            DataModel.ParentAggregatedData.SlabTextureData.TextureChanged -= OnTextureChanged;
            DataModel.SlabTextureReady -= OnSlabTextureReady;
        }

        private void OnTextureChanged()
        {
            var texture = DataModel.ParentAggregatedData.SlabTextureData.Texture;
            if (texture != null)
                ApplyTexture(texture);
        }

        private void OnSlabTextureReady(Texture2D texture)
        {
            ApplyTexture(texture);
        }

        private void ApplyTexture(Texture2D texture)
        {
            _slabMaterial.GalaxyTexture = texture;
            Mesh.Material = _slabMaterial;
            DataModel.MeshMaterial = _slabMaterial;
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var pos = DataModel.Position;
            var halfScale = DataModel.Scale / 2;
            return new BoundingBox(pos - halfScale, pos + halfScale);
        }
    }
}
