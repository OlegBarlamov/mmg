using System;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents
{
    public abstract class BillboardPrimitive<TData, TController> : RenderablePrimitive<TData, TController>
        where TData : ViewModel3D
        where TController : BillboardController<TData>
    {
        protected BillboardPrimitive(TData viewModel, string graphicsPassName)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), InitGraphicsPass(viewModel, graphicsPassName))
        {
        }

        private static TData InitGraphicsPass(TData viewModel, string graphicsPassName)
        {
            viewModel.GraphicsPassName = graphicsPassName;
            return viewModel;
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position - DataModel.Scale / 2, DataModel.Position + DataModel.Scale / 2);
        }
    }

    public abstract class BillboardController<TData> : Controller<TData>
        where TData : ViewModel3D
    {
        [NotNull] protected ICamera3DProvider Camera3DProvider { get; }

        protected virtual bool ContinuouslyUpdateRotation => true;

        protected BillboardController(
            [NotNull] TData viewModel,
            [NotNull] ICamera3DProvider camera3DProvider)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));

            SetModel(viewModel);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            UpdateBillboardRotation();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContinuouslyUpdateRotation)
                UpdateBillboardRotation();
        }

        protected void UpdateBillboardRotation()
        {
            var cameraPosition = Camera3DProvider.GetActiveCamera().GetPosition();
            DataModel.Rotation = GetBillboardRotation(DataModel.Position, cameraPosition, Vector3.Up);
        }

        protected static Matrix GetBillboardRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
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
            return Matrix.CreateRotationX(3 * MathHelper.Pi / 2) * new Matrix
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
