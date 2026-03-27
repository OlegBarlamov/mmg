using System;
using Atom.Client.Resources;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public sealed class StarAsPointViewComponent : SingleMeshComponent<StarAsPoint, StarAsPointController>
    {
        public StarAsPointViewComponent(
            [NotNull] StarAsPoint model,
            [NotNull] MainResourcePackage resourcePackage)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Textured_No_Lights")
        {
            if (resourcePackage == null) throw new ArgumentNullException(nameof(resourcePackage));

            SetDataModel(model);

            Mesh.Material = new TextureMaterial(resourcePackage.StarTexture);
            Mesh.SetPosition(DataModel.GetWorldPosition());
            Mesh.Scale = new Vector3(DataModel.AggregatedData.Power * 40);
            Mesh.Rotation = DataModel.AggregatedData.Rotation;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            Mesh.Rotation = DataModel.AggregatedData.Rotation;
            DataModel.AggregatedData.RotationChanged += OnRotationChanged;
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);

            DataModel.AggregatedData.RotationChanged -= OnRotationChanged;
        }

        private void OnRotationChanged()
        {
            Mesh.Rotation = DataModel.AggregatedData.Rotation;
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var size = Mesh.Scale;
            return new BoundingBox(DataModel.GetWorldPosition() - size / 2, DataModel.GetWorldPosition() + size / 2);
        }
    }
}
