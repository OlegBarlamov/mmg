using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public sealed class WorldMapCellContentViewComponent : SingleMeshComponent<WorldMapCellContent, WorldMapCellContentController>
    {
        public WorldMapCellContentViewComponent(WorldMapCellContent dataModel, ColorsTexturesPackage colorsTexturesPackage)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Textured_No_Lights")
        {
            SetDataModel(dataModel);

            Mesh.Position = DataModel.GetWorldPosition();
            Mesh.Scale = DataModel.Size;
            Mesh.Rotation = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Rotation;
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.RotationChanged += WorldMapCellTextureDataOnRotationChanged;

            Mesh.Material = new TextureMaterial(colorsTexturesPackage.Get(Color.Transparent));
            WorldMapCellTextureDataOnTextureChanged();
        }

        private void WorldMapCellTextureDataOnRotationChanged()
        {
            Mesh.Rotation = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Rotation;
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

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.GetWorldPosition() - DataModel.Size / 2, DataModel.GetWorldPosition() + DataModel.Size / 2);
        }
        
        private void WorldMapCellTextureDataOnTextureChanged()
        {
            var texture = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Texture; 
            if (texture != null)
                Mesh.Material = new TextureMaterial(texture);
        }
    }
}