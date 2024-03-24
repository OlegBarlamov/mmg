using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class GamepadProvider : IGamepadProvider, IUpdatable
    {
        public GamePadDeadZone GamepadDeadZone { get; set; } = GamePadDeadZone.IndependentAxes;
        
        public IPlayerGamepadProvider Gamepad1 => _gamepads[0];
        public IPlayerGamepadProvider Gamepad2 => _gamepads[1];
        public IPlayerGamepadProvider Gamepad3 => _gamepads[2];
        public IPlayerGamepadProvider Gamepad4 => _gamepads[3];

        private readonly PlayerGamepadProvider[] _gamepads = new PlayerGamepadProvider[GamePad.MaximumGamePadCount];

        public GamepadProvider()
        {
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                _gamepads[i] = new PlayerGamepadProvider((PlayerIndex)i);
            }
        }
        
        public IPlayerGamepadProvider GetGamepad(PlayerIndex index)
        {
            return GetGamepad((int) index);
        }

        public IPlayerGamepadProvider GetGamepad(int index)
        {
            return _gamepads[index];
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (var gamepad in _gamepads)
            {
                if (!gamepad.IsConnected)
                    continue;

                var newState = GamePad.GetState(gamepad.GetIndex(), GamepadDeadZone);
                gamepad.UpdateState(newState);
            }
        }
    }
}