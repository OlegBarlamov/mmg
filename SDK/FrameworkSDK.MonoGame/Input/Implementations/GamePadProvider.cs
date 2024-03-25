using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.InputManagement.Emulators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class GamePadProvider : IGamePadProvider, IMonoGameGamePadSettings, IUpdatable, IDisposable
    {
        public int MaximumGamePadCount { get; set; } = GamePadCountManagedByMonoGame;

        private static int GamePadCountManagedByMonoGame => GamePad.MaximumGamePadCount;

        public IMonoGameGamePadSettings MonoGameGamePadSettings => this;

        public GamePadDeadZone GamepadDeadZone
        {
            get => _currentGamePadDeadZone;
            set
            {
                if (_currentGamePadDeadZone != value)
                {
                    _currentGamePadDeadZone = value;
                    foreach (var monogameGamePadDataSource in _defaultDataSources)
                    {
                        monogameGamePadDataSource.GamePadDeadZone = value;
                    }
                }
            }
        }
        
        private ModuleLogger Logger { get; }

        private GamePadDeadZone _currentGamePadDeadZone = GamePadDeadZone.IndependentAxes;
        private bool _isGamePadsEnabled;
        private bool _isDisposed;

        private readonly MonogameGamePadDataSource[] _defaultDataSources = new MonogameGamePadDataSource[GamePadCountManagedByMonoGame];
        private readonly Dictionary<int, PlayerGamePadProvider> _gamePadProviders = new Dictionary<int, PlayerGamePadProvider>();
        private readonly List<PlayerGamePadProvider> _allGamePadProviders = new List<PlayerGamePadProvider>();

        public GamePadProvider(IFrameworkLogger logger)
        {
            Logger = new ModuleLogger(logger, LogCategories.Input);
            for (int i = 0; i < GamePadCountManagedByMonoGame; i++)
            {
                var dataSource = new MonogameGamePadDataSource((PlayerIndex)i, GamepadDeadZone);
                var provider = new PlayerGamePadProvider(i, dataSource, Logger);
                _defaultDataSources[i] = dataSource;
                _gamePadProviders.Add(i, provider);
                _allGamePadProviders.Add(provider);
            }
        }
        
        public void Dispose()
        {
            _isDisposed = true;

            foreach (var provider in _allGamePadProviders)
            {
                provider.DataSource.Dispose();
                provider.Dispose();
            }
        }

        public IPlayerGamePadProvider GetGamePad(int index)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(GamePadProvider));
            
            return GetOrCreatePlayerGamePadProvider(index);
        }

        public void EnableGamePads()
        {
            _isGamePadsEnabled = true;
            Logger.Info("GamePads enabled");
        }

        public void DisableGamePads()
        {
            _isGamePadsEnabled = false;
            Logger.Info("GamePads disabled");
        }

        public void ActivateEmulator(int playerIndex, IGamePadDataSource dataSource)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(GamePadProvider));
            
            var playerGamePadProvider = GetOrCreatePlayerGamePadProvider(playerIndex);
            ChangeGamePadProviderDataSource(playerGamePadProvider, dataSource);
            
            Logger.Info($"GamePad emulator {dataSource.GetMeta().DisplayName} activated for PlayerIndex {playerIndex}");
        }

        public void DeactivateEmulator(int playerIndex)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(GamePadProvider));
            
            if (_gamePadProviders.TryGetValue(playerIndex, out var provider))
            {
                var oldDataSource = provider.DataSource;
                if (provider.PlayerIndex < GamePadCountManagedByMonoGame)
                {
                    ChangeGamePadProviderDataSource(provider, _defaultDataSources[playerIndex]);
                }
                else
                {
                    ChangeGamePadProviderDataSource(provider, new EmptyGamePadDataSource());
                }
                Logger.Info($"GamePad emulator {oldDataSource.GetMeta().DisplayName} deactivated for PlayerIndex {playerIndex}");
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!_isGamePadsEnabled || _isDisposed)
                return;
                
            foreach (var gamepad in _allGamePadProviders)
            {
                gamepad.Update(gameTime);
            }
        }
        
        private PlayerGamePadProvider GetOrCreatePlayerGamePadProvider(int index)
        {
            if (_gamePadProviders.TryGetValue(index, out var provider))
                return provider;
            
            var newGamePadProvider = new PlayerGamePadProvider(index, new EmptyGamePadDataSource(), Logger);
            _gamePadProviders.Add(index, newGamePadProvider);
            _allGamePadProviders.Add(newGamePadProvider);
            return newGamePadProvider;
        }
        
        private void ChangeGamePadProviderDataSource(PlayerGamePadProvider gamePadProvider,
            IGamePadDataSource newDataSource)
        {
            gamePadProvider.ChangeSource(newDataSource, out var oldDataSource);
            DisposeDataSourceIfNeeded(oldDataSource);
        }

        private void DisposeDataSourceIfNeeded(IGamePadDataSource dataSource)
        {
            if (!(dataSource is MonogameGamePadDataSource))
            {
                dataSource.Dispose();
            }
        }
    }
}