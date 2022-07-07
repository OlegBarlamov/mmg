using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Components
{
    public sealed class WorldMapCellContentViewComponent : SingleMeshComponent<WorldMapCellContent, WorldMapCellContentController>
    {
        public WorldMapCellContentViewComponent(WorldMapCellContent dataModel, ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Textured_No_Lights")
        {
            SetDataModel(dataModel);

            var cameraPosition = camera3DProvider.GetActiveCamera().GetPosition();
            var cameraMapCellCenter = GlobalWorldMap.WorldFromMapPoint(GlobalWorldMap.MapPointFromWorld(cameraPosition));
            var objectPosition = DataModel.GetWorldPosition();
            Mesh.Position = objectPosition;
            Mesh.Scale = DataModel.Size;
            Mesh.Rotation = GetBillboardRotation(objectPosition, cameraMapCellCenter, Vector3.Up);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
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

        private Matrix GetBillboardRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
        {
            var normal = cameraPosition - objectPosition;
            normal.Normalize();
            if (normal == Vector3.Up)
                return Matrix.Identity;
            if (normal == Vector3.Down)
                return Matrix.CreateRotationX(MathHelper.Pi);
            
            var rotationAxis = Vector3.Cross(cameraUp, normal);
            rotationAxis.Normalize();
            var down = Vector3.Cross(normal, rotationAxis);
            return Matrix.CreateRotationX(-MathHelper.Pi / 2) * new Matrix
            {
                M11 = -rotationAxis.X,
                M12 = -rotationAxis.Y,
                M13 = -rotationAxis.Z,
                M14 = 0.0f,
                M21 = down.X,
                M22 = down.Y,
                M23 = down.Z,
                M24 = 0.0f,
                M31 = normal.X,
                M32 = normal.Y,
                M33 = normal.Z,
                M34 = 0.0f,
                M41 = 0f,
                M42 = 0f,
                M43 = 0f,
                M44 = 1f
            };
        }
    }
}