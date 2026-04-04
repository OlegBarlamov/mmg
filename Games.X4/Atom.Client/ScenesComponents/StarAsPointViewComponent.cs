using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarAsPointViewComponent : SingleMeshComponent<StarAsPoint, StarAsPointController>
    {
        public StarAsPointViewComponent(
            [NotNull] StarAsPoint model)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Stars")
        {
            SetDataModel(model);

            var worldPos = model.GetWorldPosition();
            Mesh.SetPosition(worldPos);
            Mesh.Scale = new Vector3(model.AggregatedData.Power * 40);
            Mesh.Material = new StarMaterial(model.AggregatedData.Color, model.AggregatedData.Power);
        }

        public void UpdateBillboardRotation(Vector3 cameraPosition)
        {
            var objectPosition = Mesh.Position;
            Mesh.Rotation = BillboardMath.GetBillboardRotation(objectPosition, cameraPosition, Vector3.Up);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(Mesh.Position - Mesh.Scale / 2, Mesh.Position + Mesh.Scale / 2);
        }
    }
}
