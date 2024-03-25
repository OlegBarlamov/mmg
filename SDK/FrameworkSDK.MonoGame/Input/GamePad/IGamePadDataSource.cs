using System;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IGamePadDataSource : IDisposable
    {
        GamePadState GetState();

        IGamePadDataSourceMeta GetMeta();

        bool SetVibration(float leftMotor, float rightMotor);
    }
}