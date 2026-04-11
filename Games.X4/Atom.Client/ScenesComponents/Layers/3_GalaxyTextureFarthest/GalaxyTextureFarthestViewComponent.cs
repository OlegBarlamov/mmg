using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class GalaxyTextureFarthestViewComponent : RenderablePrimitive<GalaxyTextureFarthestViewModel3D, GalaxyTextureFarthestController>
    {
        private readonly Vector3 _targetScale;

        public GalaxyTextureFarthestViewComponent([NotNull] GalaxyTextureFarthestViewModel3D viewModel)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), viewModel)
        {
            _targetScale = viewModel.Scale;
            viewModel.Scale = Vector3.Zero;
            Mesh.Scale = Vector3.Zero;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var aggData = DataModel.AggregatedData;
            var starViewModel = new GalaxyAsPointViewModel3D(
                DataModel.Position,
                aggData.GalaxyColor,
                1.0f,
                aggData.DiskRadius * 2f);
            AddChild(starViewModel);

            OnTextureChanged();
            aggData.TextureData.TextureChanged += OnTextureChanged;
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);

            DataModel.AggregatedData.TextureData.TextureChanged -= OnTextureChanged;
        }

        private void OnTextureChanged()
        {
            var texture = DataModel.AggregatedData.TextureData.Texture;
            if (texture != null)
            {
                var material = new TextureMaterial(texture);
                DataModel.MeshMaterial = material;
                Mesh.Material = material;
                DataModel.Scale = _targetScale;
                Mesh.Scale = _targetScale;
            }
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position - DataModel.Scale / 2, DataModel.Position + DataModel.Scale / 2);
        }
    }
}
