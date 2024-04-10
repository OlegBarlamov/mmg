using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.SceneComponents.Controllers
{
    public class CenteredCameraController : Controller
    {
        private IPositioned2D CenterTarget { get; }
        private IMutableCamera2D Camera { get; }

        public CenteredCameraController([NotNull] IPositioned2D centerTarget, [NotNull] IMutableCamera2D camera)
        {
            CenterTarget = centerTarget ?? throw new ArgumentNullException(nameof(centerTarget));
            Camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            Camera.CenterTo(CenterTarget.Position);
        }
    }
}