using System;
using Atom.Client.MacOS.Resources;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace Atom.Client.MacOS.Components
{
    [UsedImplicitly]
    public sealed class StarViewComponent : View<StarModel>
    {
        public StarModel Model { get; }
        public MainResourcePackage ResourcePackage { get; }
        public ICamera3DProvider Camera3DProvider { get; }
        public IDisplayService DisplayService { get; }

        private static readonly Vector3 DefaultStarViewSize = new Vector3(5);

        public StarViewComponent([NotNull] StarModel model, [NotNull] MainResourcePackage resourcePackage,
            [NotNull] ICamera3DProvider camera3DProvider, IDisplayService displayService)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            ResourcePackage = resourcePackage ?? throw new ArgumentNullException(nameof(resourcePackage));
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
            DisplayService = displayService;

            BoundingBox = new BoundingBox(Model.World - DefaultStarViewSize / 2, Model.World + DefaultStarViewSize / 2);
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            var leftTopStarOnScreenPoint = DisplayService.GraphicsDevice.Viewport.Project(Model.World + (-GetRightVector() + GetUpVector()) * DefaultStarViewSize / 3,
                                                 Camera3DProvider.GetActiveCamera().GetProjection(),
                                                 Camera3DProvider.GetActiveCamera().GetView(),
                                                 Matrix.Identity);
            var endStarOnScreenPoint = DisplayService.GraphicsDevice.Viewport.Project(Model.World + (GetRightVector() - GetUpVector()) * DefaultStarViewSize / 3,
                Camera3DProvider.GetActiveCamera().GetProjection(),
                Camera3DProvider.GetActiveCamera().GetView(),
                Matrix.Identity);

            var rec = new Rectangle(leftTopStarOnScreenPoint.EmitZ().ToPoint(),
                (endStarOnScreenPoint - leftTopStarOnScreenPoint).EmitZ().ToPoint());
            
            context.Draw(ResourcePackage.StarTexture, rec, Color.White);
        }

        private Vector3 GetRightVector()
        {
            var camera = (DirectionalCamera3D)Camera3DProvider.GetActiveCamera();
            var direction = camera.Target - camera.Position;
            var right = Vector3.Cross(direction, Vector3.Up);
            right.Normalize();
            return right;
        }

        private Vector3 GetUpVector()
        {
            var camera = (DirectionalCamera3D)Camera3DProvider.GetActiveCamera();
            var up = camera.Up;
            up.Normalize();
            return up;
        }
    }
}