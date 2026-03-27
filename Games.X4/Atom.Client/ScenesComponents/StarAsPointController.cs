using System;
using Atom.Client.Services;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarAsPointController : Controller<StarAsPoint>
    {
        [NotNull] private IPlayerProvider PlayerProvider { get; }

        public StarAsPointController(
            [NotNull] StarAsPoint model,
            [NotNull] IPlayerProvider playerProvider)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            PlayerProvider = playerProvider ?? throw new ArgumentNullException(nameof(playerProvider));

            SetModel(model);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            UpdateBillboardRotation();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateBillboardRotation();
        }

        private void UpdateBillboardRotation()
        {
            var cameraPosition = PlayerProvider.GetPlayerPosition();
            DataModel.AggregatedData.Rotation =
                GetBillboardRotation(DataModel.GetWorldPosition(), cameraPosition, Vector3.Up);
        }

        private static Matrix GetBillboardRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
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
