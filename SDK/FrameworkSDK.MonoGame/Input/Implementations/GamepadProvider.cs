using System.Collections.Generic;
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
        private readonly Dictionary<int, IGamepadEmulator> _emulators = new Dictionary<int, IGamepadEmulator>(); 

        public GamepadProvider()
        {
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                _gamepads[i] = new PlayerGamepadProvider((PlayerIndex)i);
            }
        }

        public void ActivateEmulator(IGamepadEmulator emulator)
        {
            _emulators[(int) emulator.GetIndex()] = emulator;
        }

        public void DeactivateEmulator(IGamepadEmulator emulator)
        {
            _emulators.Remove((int) emulator.GetIndex());
        }

        public IPlayerGamepadProvider GetGamepad(PlayerIndex index)
        {
            return GetGamepad((int) index);
        }

        public IPlayerGamepadProvider GetGamepad(int index)
        {
            return (IPlayerGamepadProvider)FindEmulator(index) ?? _gamepads[index];
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (var gamepad in _gamepads)
            {
                if (_emulators.TryGetValue((int)gamepad.GetIndex(), out var emulator))
                {
                    emulator.Update(gameTime);
                    continue;
                }
                
                if (!gamepad.IsConnected)
                    continue;

                var newState = GamePad.GetState(gamepad.GetIndex(), GamepadDeadZone);
                gamepad.UpdateState(newState);
            }
        }

        private IGamepadEmulator FindEmulator(int index)
        {
            return _emulators.TryGetValue(index, out var emulator) ? emulator : null;
        }
    }
}