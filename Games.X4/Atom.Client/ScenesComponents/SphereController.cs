using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class SphereController : Controller<PlanetSystemFarthest>
    {
        private readonly ICamera3DProvider _camera3DProvider;

        public SphereController(
            [NotNull] PlanetSystemFarthest model,
            [NotNull] ICamera3DProvider camera3DProvider)
        {
            _camera3DProvider = camera3DProvider;
            SetModel(model);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (((IController)this).View is SphereViewComponent view)
            {
                var cameraPos = _camera3DProvider.GetActiveCamera().GetPosition();
                view.UpdateBillboardRotation(cameraPos);
            }
        }
    }
}
