using System;
using System.Threading;
using Atom.Client.MacOS.Resources;
using Atom.Client.MacOS.Scenes;
using Atom.Client.MacOS.Services;
using Atom.Client.MacOS.Services.Implementations;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
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
        private IAstronomicMapGenerator AstronomicMapGenerator { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }

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
            [NotNull] IAstronomicMapGenerator astronomicMapGenerator,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection)
        {
            ScenesResolverHolder = scenesResolverHolder ?? throw new ArgumentNullException(nameof(scenesResolverHolder));
            LoadingSceneResources = loadingSceneResources ?? throw new ArgumentNullException(nameof(loadingSceneResources));
            DefaultConsoleManipulator = defaultConsoleManipulator ?? throw new ArgumentNullException(nameof(defaultConsoleManipulator));
            MainSceneDataModel = mainSceneDataModel ?? throw new ArgumentNullException(nameof(mainSceneDataModel));
            MainResourcePackage = mainResourcePackage ?? throw new ArgumentNullException(nameof(mainResourcePackage));
            ResourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
            AstronomicMapGenerator = astronomicMapGenerator ?? throw new ArgumentNullException(nameof(astronomicMapGenerator));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
            
            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<string>("scene", "Switch current scene",
                scene =>
                {
                    if (scene == _mainScene.Name)
                        _currentScene = _mainScene;
                    if (scene == _loadingScene.Name)
                        _currentScene = _loadingScene;
                }));
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

            _loadingScene = (LoadingScene) ScenesResolver.ResolveScene(LoadingSceneResources);
            _mainScene = (MainScene) ScenesResolver.ResolveScene(MainSceneDataModel);
            _currentScene = _loadingScene;

            AstronomicMapGenerator.GenerateMapAsync(Point3D.Zero,
                    new Point3D(MainSceneDataModel.AstronomicMapViewRadius), _appLifeTimeTokenSource.Token)
                .ContinueWith(task =>
                {
                    MainSceneDataModel.Initialize(task.Result);
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
    }
}