using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using X4World.Objects;

namespace Atom.Client.Components
{
    public sealed class WorldMapCellContentViewComponent : BillboardPrimitive<WorldMapCellContent, WorldMapCellContentController>
    {
        public WorldMapCellContentViewComponent(WorldMapCellContent model)
            : base(model, GraphicsPasses.TexturedNoLights)
        {
            DataModel.Scale = model.Size;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            WorldMapCellTextureDataOnTextureChanged();
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.TextureChanged += WorldMapCellTextureDataOnTextureChanged;
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.TextureChanged -= WorldMapCellTextureDataOnTextureChanged;
        }

        private void WorldMapCellTextureDataOnTextureChanged()
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
