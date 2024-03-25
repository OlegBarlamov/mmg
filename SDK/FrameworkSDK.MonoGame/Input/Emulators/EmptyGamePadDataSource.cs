using FrameworkSDK.Common;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Emulators
{
    internal class EmptyGamePadDataSource : IGamePadDataSource
    {
        private readonly IGamePadDataSourceMeta _emptyMeta = new CustomGamePadDataSourceMeta
        {
            DisplayName = "NoGamePad",
            GamePadType = GamePadType.Unknown,
            Identifier = Hash.Generate(HashType.SmallGuid).ToString()
        };

        public GamePadState GetState()
        {
            return GamePadState.Default;
        }

        public IGamePadDataSourceMeta GetMeta()
        {
            return _emptyMeta;
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return false;
        }

        public void Dispose()
        {
        }
    }
}