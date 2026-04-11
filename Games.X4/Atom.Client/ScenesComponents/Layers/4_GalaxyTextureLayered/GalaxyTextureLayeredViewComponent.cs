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
    public sealed class GalaxyTextureLayeredViewComponent : RenderablePrimitive<GalaxyTextureLayeredViewModel3D, GalaxyTextureLayeredController>
    {
        public GalaxyTextureLayeredViewComponent([NotNull] GalaxyTextureLayeredViewModel3D viewModel)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), viewModel)
        {
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            var aggData = DataModel.AggregatedData;
            var starViewModel = new GalaxyAsPointViewModel3D(
                DataModel.Position,
                aggData.GalaxyColor,
                0.5f,
                aggData.DiskRadius);
            AddChild(starViewModel);

            aggData.TextureData.TextureChanged += OnTextureChanged;
            OnTextureChanged();
        }

        protected override void OnDetached(SceneBase scene)
        {
            DataModel.AggregatedData.TextureData.TextureChanged -= OnTextureChanged;
            base.OnDetached(scene);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position - DataModel.Scale / 2, DataModel.Position + DataModel.Scale / 2);
        }

        private void OnTextureChanged()
        {
            var texture = DataModel.AggregatedData.TextureData.Texture;
            if (texture != null)
            {
                Mesh.Material = new TextureMaterial(texture);
            }
        }
    }
}
