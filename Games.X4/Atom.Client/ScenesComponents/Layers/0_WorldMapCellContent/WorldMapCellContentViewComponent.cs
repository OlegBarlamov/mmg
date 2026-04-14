using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;

namespace Atom.Client.Components
{
    public sealed class WorldMapCellContentViewComponent : BillboardPrimitive<WorldMapCellContentViewModel3D, WorldMapCellContentController>
    {
        public WorldMapCellContentViewComponent(WorldMapCellContentViewModel3D viewModel)
            : base(viewModel, GraphicsPasses.TexturedNoLights)
        {
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            OnTextureChanged();
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.TextureChanged += OnTextureChanged;
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.TextureChanged -= OnTextureChanged;
        }

        private void OnTextureChanged()
        {
            var texture = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Texture; 
            if (texture != null)
            {
                var material = new TextureMaterial(texture);
                DataModel.MeshMaterial = material;
                Mesh.Material = material;
            }
        }
    }
}
