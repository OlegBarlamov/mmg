using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    internal class PlayerGamePadProvider : IPlayerGamePadProvider, IUpdatable, IDisposable
    {
        public event Action<GamePadEventHandlerArgs> Connected;
        public event Action<GamePadEventHandlerArgs> Disconnected;

        public bool IsConnected => Current.IsConnected;
        public GamePadState Current { get; private set; }
        public GamePadState Previous { get; private set; }

        public GamePadThumbSticks ThumbSticks => Current.ThumbSticks;

        public GamePadTriggers Triggers => Current.Triggers;

        public bool IsLeftTriggerPressed => Triggers.Left > 0;

        public bool IsRightTriggerPressed => Triggers.Right > 0;
        
        public int PlayerIndex { get; }
        public IGamePadDataSource DataSource { get; private set; }
        private ModuleLogger Logger { get; }

        private IGamePadDataSource _connectedDataSource;

        public PlayerGamePadProvider(int playerIndex, [NotNull] IGamePadDataSource gamePadDataSource,
            [NotNull] ModuleLogger logger)
        {
            PlayerIndex = playerIndex;
            DataSource = gamePadDataSource ?? throw new ArgumentNullException(nameof(gamePadDataSource));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void Dispose()
        {
            Connected = null;
            Disconnected = null;
        }

        public void ChangeSource([NotNull] IGamePadDataSource newDataSource, out IGamePadDataSource oldDataSource)
        {
            if (newDataSource == null) throw new ArgumentNullException(nameof(newDataSource));
            oldDataSource = DataSource;
            DataSource = newDataSource;
        }

        public void Update(GameTime gameTime)
        {
            Previous = Current;
            Current = DataSource.GetState();;

            if (Current.IsConnected && !Previous.IsConnected)
            {
                Logger.Info($"GamePad {DataSource.GetMeta().DisplayName} connected for PlayerIndex {PlayerIndex}");
                Connected?.Invoke(new GamePadEventHandlerArgs(PlayerIndex, DataSource));
                _connectedDataSource = DataSource;
            }
            else if (!Current.IsConnected && Previous.IsConnected)
            {
                Logger.Info($"GamePad {DataSource.GetMeta().DisplayName} disconnected for PlayerIndex {PlayerIndex}");
                Disconnected?.Invoke(new GamePadEventHandlerArgs(PlayerIndex, _connectedDataSource));
            }
        }
        
        public int GetIndex()
        {
            return PlayerIndex;
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return DataSource.SetVibration(leftMotor, rightMotor);
        }

        public bool IsButtonDown(Buttons button)
        {
            return Current.IsButtonDown(button);
        }
        
        public bool IsButtonUp(Buttons button)
        {
            return Current.IsButtonUp(button);
        }

        public bool IsButtonPressedOnce(Buttons button)
        {
            return IsButtonDown(button) && Previous.IsButtonUp(button);
        }

        public bool IsButtonReleasedOnce(Buttons button)
        {
            return IsButtonUp(button) && Previous.IsButtonDown(button);
        }

        public IGamePadDataSourceMeta GetGamePadMeta()
        {
            return DataSource.GetMeta();
        }
    }
}