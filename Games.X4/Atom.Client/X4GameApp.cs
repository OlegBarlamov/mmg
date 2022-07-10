using System;
using System.Threading;
using System.Threading.Tasks;
using Atom.Client.Resources;
using Atom.Client.Scenes;
using Atom.Client.Services.Implementations;
using Console.Core;
using Console.Core.Models;
using Console.FrameworkAdapter;
using Console.InGame;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using NetExtensions.Geometry;
using NetExtensions.Helpers;
using X4World.Generation;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client
{
    [UsedImplicitly]
    public class X4GameApp : GameApp
    {
        protected override SceneBase CurrentScene => _currentScene;

        private IScenesResolver ScenesResolver => ScenesResolverHolder.ScenesResolver;
        [NotNull] private ScenesResolverHolder ScenesResolverHolder { get; }
        private LoadingSceneResources LoadingSceneResources { get; }
        private DefaultConsoleManipulator DefaultConsoleManipulator { get; }
        private MainSceneDataModel MainSceneDataModel { get; }
        private MainResourcePackage MainResourcePackage { get; }
        private IResourcesService ResourcesService { get; }
        public IRandomService RandomService { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }
        public IRandomSeedProvider RandomSeedProvider { get; }
        public IConsoleController ConsoleController { get; }

        private SceneBase _currentScene;
        private LoadingScene _loadingScene;
        private MainScene _mainScene;
        
        private readonly CancellationTokenSource _appLifeTimeTokenSource = new CancellationTokenSource();

        public X4GameApp(
            [NotNull] ScenesResolverHolder scenesResolverHolder,
            [NotNull] LoadingSceneResources loadingSceneResources,
            [NotNull] DefaultConsoleManipulator defaultConsoleManipulator,
            [NotNull] MainSceneDataModel mainSceneDataModel,
            [NotNull] MainResourcePackage mainResourcePackage,
            [NotNull] IResourcesService resourcesService,
            [NotNull] IRandomService randomService,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection,
            [NotNull] IRandomSeedProvider randomSeedProvider,
            [NotNull] IConsoleController consoleController)
        {
            ScenesResolverHolder = scenesResolverHolder ?? throw new ArgumentNullException(nameof(scenesResolverHolder));
            LoadingSceneResources = loadingSceneResources ?? throw new ArgumentNullException(nameof(loadingSceneResources));
            DefaultConsoleManipulator = defaultConsoleManipulator ?? throw new ArgumentNullException(nameof(defaultConsoleManipulator));
            MainSceneDataModel = mainSceneDataModel ?? throw new ArgumentNullException(nameof(mainSceneDataModel));
            MainResourcePackage = mainResourcePackage ?? throw new ArgumentNullException(nameof(mainResourcePackage));
            ResourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
            RandomSeedProvider = randomSeedProvider ?? throw new ArgumentNullException(nameof(randomSeedProvider));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
        }

        protected override void Dispose()
        {
            _appLifeTimeTokenSource.Dispose();
            
            base.Dispose();
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
            
            ResourcesService.LoadPackage(MainResourcePackage);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ((InGameConsoleController)ConsoleController).AddFilter(ConsoleLogLevel.Error);
            ((InGameConsoleController)ConsoleController).AddFilter(ConsoleLogLevel.Critical);
            ((InGameConsoleController)ConsoleController).AddFilter(ConsoleLogLevel.Warning);
            //((InGameConsoleController)ConsoleController).AddFilter(ConsoleLogLevel.Information);
            
            _loadingScene = (LoadingScene) ScenesResolver.ResolveScene(LoadingSceneResources);
            _currentScene = _loadingScene;

            GenerateMapAsync(_appLifeTimeTokenSource.Token)
                .ContinueWith(task =>
                {
                    MainSceneDataModel.Initialize(task.Result);
                    _mainScene = (MainScene) ScenesResolver.ResolveScene(MainSceneDataModel);
                    _currentScene = _mainScene;
                })
                .ConfigureAwait(true);
        }

        protected override void OnContentUnloading()
        {
            _appLifeTimeTokenSource.Cancel();
            
            base.OnContentUnloading();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            DefaultConsoleManipulator.Update(gameTime);
        }

        private Task<GlobalWorldMap> GenerateMapAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                
               var cellGenerator = new WorldMapCellGenerator(RandomService);
               var worldMapGenerator = new WorldMapGenerator(cellGenerator, RandomSeedProvider);
               return (GlobalWorldMap)worldMapGenerator.Generate();
               
            }, TaskCreationOptions.LongRunning);
        }
    }
}