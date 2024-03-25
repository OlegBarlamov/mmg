using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    internal class MonogameGamePadDataSource : IGamePadDataSource
    {
        public PlayerIndex PlayerIndex { get; }
        public GamePadDeadZone GamePadDeadZone { get; set; }

        public MonogameGamePadDataSource(PlayerIndex playerIndex, GamePadDeadZone gamePadDeadZone)
        {
            PlayerIndex = playerIndex;
            GamePadDeadZone = gamePadDeadZone;
        }
        
        public void Dispose()
        {
        }

        public bool IsConnected { get; private set; }

        public GamePadState GetState()
        {
            var state = GamePad.GetState(PlayerIndex, GamePadDeadZone);
            IsConnected = state.IsConnected;
            return state;
        }

        public IGamePadDataSourceMeta GetMeta()
        {
            return new MonoGameGamePadDataSourceMeta(GamePad.GetCapabilities(PlayerIndex));
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return GamePad.SetVibration(PlayerIndex, leftMotor, rightMotor);
        }
    }
}