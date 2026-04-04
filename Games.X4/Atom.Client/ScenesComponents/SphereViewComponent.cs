using Atom.Client.Resources;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class SphereViewComponent : SingleMeshComponent<PlanetSystemFarthest, SphereController>
    {
        public SphereViewComponent(
            [NotNull] PlanetSystemFarthest model,
            [NotNull] MainResourcePackage resourcePackage)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Textured_No_Lights")
        {
            SetDataModel(model);

            var worldPos = model.GetWorldPosition();
            Mesh.SetPosition(worldPos);
            Mesh.Scale = new Vector3(model.AggregatedData.StarData.Power * 50f);
            var material = new TextureMaterial(resourcePackage.GalaxyTexture);
            Mesh.Material = material;
        }

        public void UpdateBillboardRotation(Vector3 cameraPosition)
        {
            var objectPosition = Mesh.Position;
            Mesh.Rotation = Atom.Client.Graphics.BillboardMath.GetBillboardRotation(objectPosition, cameraPosition, Vector3.Up);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(Mesh.Position - Mesh.Scale / 2, Mesh.Position + Mesh.Scale / 2);
        }
    }
}
