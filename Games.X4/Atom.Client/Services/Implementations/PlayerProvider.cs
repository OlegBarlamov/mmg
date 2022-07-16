using System;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.Services.Implementations
{
    public class PlayerProvider : IPlayerProvider
    {
        public ICamera3DProvider Camera3DProvider { get; }

        private Vector3 _position;

        public PlayerProvider([NotNull] ICamera3DProvider camera3DProvider)
        {
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
        }
        
        public Vector3 GetPlayerPosition()
        {
            return _position;
        }

        public void Update(GameTime gameTime)
        {
            if (!DebugServicesOnlyForDebug.DebugVariablesService.GetValue<bool>("free_camera"))
            {
                _position = Camera3DProvider.GetActiveCamera().GetPosition();
            }
        }
    }
}