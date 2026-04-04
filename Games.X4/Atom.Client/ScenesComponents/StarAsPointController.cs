using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class StarAsPointController : Controller<StarAsPoint>
    {
        private readonly ICamera3DProvider _camera3DProvider;

        public StarAsPointController(
            [NotNull] StarAsPoint model,
            [NotNull] ICamera3DProvider camera3DProvider)
        {
            _camera3DProvider = camera3DProvider;
            SetModel(model);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (((IController)this).View is StarAsPointViewComponent view)
            {
                var cameraPos = _camera3DProvider.GetActiveCamera().GetPosition();
                view.UpdateBillboardRotation(cameraPos);
            }
        }
    }
}
