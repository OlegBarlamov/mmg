using System;
using System.Collections.Generic;
using System.Threading;
using Atom.Client.Controllers;
using Atom.Client.Graphics;
using Atom.Client.Services;
using Atom.Client.Components;
using Console.Core;
using Console.Core.CommandExecution;
using Console.Core.Commands;
using FrameworkSDK.Common;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.SceneComponents.Controllers;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World;
using X4World.Objects;

namespace Atom.Client.Scenes
{
    public class MainScene : Scene
    {
        private const bool EnableDebugBoundingBoxes = true;

        private MainSceneDataModel DataModel { get; }
        public IInputService InputService { get; }
        private ICamera3DService Camera3DService { get; }
        public IRandomService RandomService { get; }
        private IConsoleController ConsoleController { get; }
        private IDebugInfoService DebugInfoService { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }
        private IMainUpdatesTasksProcessor MainUpdatesTasksProcessor { get; }
        private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        public ColorsTexturesPackage ColorsTexturesPackage { get; }
        public IFrameworkLogger Logger { get; }
        public IDetailsGeneratorProvider DetailsGeneratorProvider { get; }
        public IDisplayService DisplayService { get; }
        public IPlayerProvider PlayerProvider { get; }

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;
        private readonly IGlobalWorldMapController _globalWorldMapController;
        private readonly IWrappedObjectsController _wrappedObjectsController;
        private readonly Dictionary<IWrappedDetails, ViewModel3D> _viewModels = new Dictionary<IWrappedDetails, ViewModel3D>();
        private readonly Dictionary<object, IView> _viewByDataModel = new Dictionary<object, IView>();
        private readonly LodStatsComponentData _lodStats = new LodStatsComponentData();

        private IWrappedDetails _selectedObject;
        private DrawLabelComponentDataModel _pickLabel;

        private DebugBoundingBoxesView _debugBBView;
        private IGraphicsPipelineAction _debugBBAction;

        private BasicEffect _texturesShader;
        private BasicEffect _texturesShaderNoLights;
        private BasicEffect _coloredShader;
        private BasicEffect _cloudShader;
        private Graphics.StarEffect _starShader;
        private Graphics.GalaxySlabEffect _galaxySlabShader;

        public MainScene(
            MainSceneDataModel model,
            [NotNull] IInputService inputService,
            ICamera3DService camera3DService,
            [NotNull] IRandomService randomService,
            IConsoleController consoleController,
            IDebugInfoService debugInfoService,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection,
            [NotNull] IMainUpdatesTasksProcessor mainUpdatesTasksProcessor,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] ColorsTexturesPackage colorsTexturesPackage,
            [NotNull] IFrameworkLogger logger,
            [NotNull] IDetailsGeneratorProvider detailsGeneratorProvider,
            [NotNull] IDisplayService displayService,
            [NotNull] IPlayerProvider playerProvider
        )
            :base(nameof(MainScene))
        {
            DataModel = model;
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
            MainUpdatesTasksProcessor = mainUpdatesTasksProcessor ?? throw new ArgumentNullException(nameof(mainUpdatesTasksProcessor));
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DetailsGeneratorProvider = detailsGeneratorProvider ?? throw new ArgumentNullException(nameof(detailsGeneratorProvider));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            PlayerProvider = playerProvider ?? throw new ArgumentNullException(nameof(playerProvider));

            Camera3DService.SetActiveCamera(_camera);

            _cameraController = new FirstPersonCameraController(inputService, _camera, DebugInfoService, DisplayService);
            _globalWorldMapController = new GlobalWorldMapController(DataModel.GlobalWorldMap, debugInfoService);
            _globalWorldMapController.CellRevealed += GlobalWorldMapControllerOnCellRevealed;
            _globalWorldMapController.CellHidden += GlobalWorldMapControllerOnCellHidden;
            _wrappedObjectsController = new WrappedObjectsController(DetailsGeneratorProvider);
            _wrappedObjectsController.ObjectRevealed += WrappedObjectsControllerOnObjectRevealed;
            _wrappedObjectsController.ObjectsRevealedBatch += WrappedObjectsControllerOnObjectsRevealedBatch;
            _wrappedObjectsController.ObjectsHidden += WrappedObjectsControllerOnObjectsHidden;

            if (EnableDebugBoundingBoxes)
            {
                _debugBBView = new DebugBoundingBoxesView(GraphicsPasses.DebugBoundingBoxes);
            }
        }

        private void WrappedObjectsControllerOnObjectsHidden(IReadOnlyList<IWrappedDetails> objects)
        {
            foreach (var obj in objects)
            {
                if (_viewModels.TryGetValue(obj, out var viewModel))
                {
                    RemoveView(viewModel);
                    viewModel.Dispose();
                    _viewModels.Remove(obj);
                    TrackLayerCount(obj, -1);
                }
                
            }
        }

        private void WrappedObjectsControllerOnObjectRevealed(IWrappedDetails obj)
        {
            MainUpdatesTasksProcessor.EnqueueTask(new SimpleDelayedTask(g =>
            {
                if (!_wrappedObjectsController.IsObjectRevealed(obj))
                    return;

                RevealObject(obj);
            }, CancellationToken.None));
        }

        private void WrappedObjectsControllerOnObjectsRevealedBatch(IReadOnlyList<IWrappedDetails> details)
        {
            if (details.Count == 0) return;

            var parent = details[0].Parent;
            var actions = new List<Action>(details.Count);
            foreach (var obj in details)
            {
                actions.Add(() =>
                {
                    if (_wrappedObjectsController.IsObjectRevealed(obj))
                        RevealObject(obj);
                });
            }

            MainUpdatesTasksProcessor.EnqueueTask(new BatchDelayedTask(
                actions,
                () => parent != null && !_wrappedObjectsController.IsObjectUnwrapped(parent)));
        }

        private void RevealObject(IWrappedDetails obj)
        {
            if (_viewModels.ContainsKey(obj))
                return;

            var viewModel = CreateViewModel(obj);
            if (viewModel == null)
                return;

            _viewModels[obj] = viewModel;
            AddView(viewModel);
            TrackLayerCount(obj, 1);
        }

        private void TrackLayerCount(IWrappedDetails obj, int delta)
        {
            _lodStats.Track(obj.LayerName, delta);
        }

        private ViewModel3D CreateViewModel(IWrappedDetails obj)
        {
            switch (obj)
            {
                case WorldMapCellContent cell:
                    return new WorldMapCellContentViewModel3D(cell);
                case GalaxiesBatch galaxiesBatch:
                    return new GalaxiesBatchViewModel3D(galaxiesBatch);
                case GalaxyAsPoint galaxy:
                    return new GalaxyAsPointViewModel3D(galaxy);
                case GalaxyTextureFarthest gt:
                    return new GalaxyTextureFarthestViewModel3D(gt);
                case GalaxyTextureLayered gtl:
                    return new GalaxyTextureLayeredViewModel3D(gtl);
                case GalaxySectorTexture sectorTexture:
                    return new GalaxySectorCloudViewModel3D(sectorTexture);
                case GalaxySector sector:
                    return new GalaxySectorViewModel3D(sector);
                case GalaxySectorChunk chunk:
                    return new GalaxySectorChunkViewModel3D(chunk);
                case StarSystemsBatch batch:
                    return new StarSystemsBatchViewModel3D(batch);
                case StarSystemAsPoint sp:
                    return new StarSystemAsPointViewModel3D(sp);
                case StarSystemAsLightPoint slp:
                    return new StarSystemAsLightPointViewModel3D(slp);
                default:
                    return null;
            }
        }

        private void GlobalWorldMapControllerOnCellHidden(WorldMapCellContent cell)
        {
            _wrappedObjectsController.RemoveWrappedObject(cell);
        }

        private void GlobalWorldMapControllerOnCellRevealed(WorldMapCellContent cell)
        {
            if (!cell.IsDetailsGenerated)
                DetailsGeneratorProvider.GetGenerator(cell).Generate(cell);

            _wrappedObjectsController.AddWrappedObject(cell);
        }

        protected override void Initialize()
        {
            AddView(new DebugInfoComponentData
            {
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
                GraphicsPassName = GraphicsPasses.Debug
            });

            AddView(new FpsCounterComponentData
            {
                GraphicsPassName = GraphicsPasses.Debug,
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                Position = new Vector2(DisplayService.PreferredBackBufferWidth - 100, 20),
            });

            _lodStats.Font = DataModel.MainResourcePackage.DebugInfoFont;
            _lodStats.Position = new Vector2(DisplayService.PreferredBackBufferWidth - 20, 50);
            AddView(_lodStats);

            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<float, float, float>("pos", "Set camera position",
                (x, y, z) =>
                {
                    _camera.Position = new Vector3(x, y, z);
                }));
            
            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<bool>("free_cam", "Enable or disable free camera mode",
                value =>
                {
                    DebugServicesOnlyForDebug.DebugVariablesService.SetValue("free_camera",value);
                }));

            if (EnableDebugBoundingBoxes)
            {
                ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<bool>("debug_bb", "Toggle debug bounding boxes",
                    value =>
                    {
                        var attached = ((ISceneComponent)_debugBBView).OwnedScene != null;
                        if (value && !attached)
                            AddView(_debugBBView);
                        else if (!value && attached)
                            RemoveView(_debugBBView);
                    }));
            }

            ConsoleController.ConsoleShowed += OnConsoleShowed;
            ConsoleController.ConsoleHidden += OnConsoleHidden;

            _pickLabel = new DrawLabelComponentDataModel
            {
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                Text = string.Empty,
                Color = Color.Yellow,
                GraphicsPassName = GraphicsPasses.Debug
            };
            AddView(_pickLabel);
        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);
                PlayerProvider.Update(gameTime);

                _selectedObject = null;
                _pickLabel.Text = string.Empty;
            }
            else if (InputService.Mouse.LeftButtonPressedOnce)
            {
                ProcessObjectPicking();
            }

            var playerPosition = PlayerProvider.GetPlayerPosition();
            _globalWorldMapController.Update(playerPosition, gameTime);
            _wrappedObjectsController.Update(playerPosition, gameTime);

            MainUpdatesTasksProcessor.Update(gameTime);
            _lodStats.PendingTasks = MainUpdatesTasksProcessor.PendingTasksCount;
            _lodStats.TotalViews = _viewModels.Count;
        }

        private void OnConsoleShowed() => InputService.Mouse.IsMouseVisible = true;
        private void OnConsoleHidden() => InputService.Mouse.IsMouseVisible = false;

        private void ProcessObjectPicking()
        {
            var viewport = GameHeartServices.GraphicsDeviceManager.GraphicsDevice.Viewport;
            var ray = _camera.CreatePickRay(InputService.Mouse.Position, viewport);

            IWrappedDetails closest = null;
            var closestDistance = float.MaxValue;

            foreach (var kvp in _viewModels)
            {
                var obj = kvp.Key;

                if (_wrappedObjectsController.IsObjectUnwrapped(obj))
                    continue;

                if (!_viewByDataModel.TryGetValue(kvp.Value, out var view))
                    continue;

                if (view.BoundingBox == null)
                    continue;

                var distance = ray.Intersects(view.BoundingBox.Value);
                if (!distance.HasValue)
                    continue;

                var dist = float.IsNaN(distance.Value) ? 0f : distance.Value;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = obj;
                }
            }

            _selectedObject = closest;

            if (_selectedObject != null)
            {
                _pickLabel.Text = _selectedObject.Name;
                _pickLabel.Position = InputService.Mouse.Position.ToVector2() + new Vector2(15, 15);
            }
            else
            {
                _pickLabel.Text = string.Empty;
            }
        }

        protected override void OnViewAttached(IView view)
        {
            _viewByDataModel[view.DataModel] = view;

            _debugBBView?.TrackView(view);
        }

        protected override void OnViewDetached(IView view)
        {
            _viewByDataModel.Remove(view.DataModel);

            _debugBBView?.UntrackView(view);
        }

        public override void Dispose()
        {
            base.Dispose();

            ConsoleController.ConsoleShowed -= OnConsoleShowed;
            ConsoleController.ConsoleHidden -= OnConsoleHidden;

            _texturesShader?.Dispose();
            _coloredShader?.Dispose();
            _texturesShaderNoLights?.Dispose();
            _cloudShader?.Dispose();
            _starShader?.Dispose();
            _galaxySlabShader?.Dispose();
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            _texturesShader = new BasicEffect(GameHeartServices.GraphicsDeviceManager.GraphicsDevice)
            {
                VertexColorEnabled = false, TextureEnabled = true
            };
            _texturesShader.EnableDefaultLighting();

            _coloredShader = new BasicEffect(GameHeartServices.GraphicsDeviceManager.GraphicsDevice)
            {
                TextureEnabled = false,
                VertexColorEnabled = true
            };
            
            _texturesShaderNoLights = new BasicEffect(GameHeartServices.GraphicsDeviceManager.GraphicsDevice)
            {
                VertexColorEnabled = false, TextureEnabled = true
            };

            _cloudShader = new BasicEffect(GameHeartServices.GraphicsDeviceManager.GraphicsDevice)
            {
                VertexColorEnabled = true, TextureEnabled = true, LightingEnabled = false
            };

            _starShader = DataModel.MainResourcePackage.StarShader;
            _galaxySlabShader = DataModel.MainResourcePackage.GalaxySlabShader;

            var vertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionColor.VertexDeclaration, 100);
            var indexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(200);
            
            var vertexBuffer2 = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionNormalTexture.VertexDeclaration, 1000);
            var indexBuffer2 = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(5000);
            
            var vertexBuffer3 = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionNormalTexture.VertexDeclaration, 1000);
            var indexBuffer3 = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(5000);

            var starVertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionNormalTexture.VertexDeclaration, 6000);
            var starIndexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(9000);

            var coloredStarVertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionColor.VertexDeclaration, 512000);
            var coloredStarIndexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(1024000);

            var cloudVertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionColorTexture.VertexDeclaration, 48000);
            var cloudIndexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(72000);

            var slabVertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionNormalTexture.VertexDeclaration, 1000);
            var slabIndexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(5000);

            if (EnableDebugBoundingBoxes)
            {
                var debugBBVertexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateVertexBugger(VertexPositionColor.VertexDeclaration, 50000);
                var debugBBIndexBuffer = graphicsPipelineBuilder.VideoBuffersFactoryService.CreateIndexBuffer(100000);
                _debugBBAction = new SimpleRenderInstancedMeshes<VertexPositionColor>(GraphicsPasses.DebugBoundingBoxes, _coloredShader, debugBBVertexBuffer, debugBBIndexBuffer);
            }

            var pipelineBuilder = graphicsPipelineBuilder
                .Clear(Color.Black)
                .SetRenderingConfigs(BlendState.AlphaBlend, DepthStencilState.Default, RasterizerStates.Default)
                .ApplyActiveCameraToShader(_coloredShader)
                .RenderGrouped<VertexPositionColor>(_coloredShader, vertexBuffer, indexBuffer, GraphicsPasses.Grouped);

            if (EnableDebugBoundingBoxes)
                pipelineBuilder = pipelineBuilder.AddAction(_debugBBAction);

            return pipelineBuilder
                .SetRenderingConfigs(BlendState.AlphaBlend, DepthStencilState.DepthRead, RasterizerStates.NoCull)
                .ApplyActiveCameraToShader(_texturesShaderNoLights)
                .RenderGrouped<VertexPositionNormalTexture>(_texturesShaderNoLights, vertexBuffer3, indexBuffer3, GraphicsPasses.TexturedNoLights)
                .SetRenderingConfigs(BlendState.AlphaBlend, DepthStencilState.DepthRead, RasterizerStates.NoCull)
                .ApplyActiveCameraToShader(_galaxySlabShader)
                .RenderGrouped<VertexPositionNormalTexture>(_galaxySlabShader, slabVertexBuffer, slabIndexBuffer, GraphicsPasses.GalaxySlab)
                .SetRenderingConfigs(BlendState.Additive, DepthStencilState.DepthRead, RasterizerStates.NoCull)
                .ApplyActiveCameraToShader(_cloudShader)
                .RenderGrouped<VertexPositionColorTexture>(_cloudShader, cloudVertexBuffer, cloudIndexBuffer, GraphicsPasses.CloudSprites)
                .SetRenderingConfigs(BlendState.AlphaBlend, DepthStencilState.Default, RasterizerStates.Default)
                .ApplyActiveCameraToShader(_texturesShader)
                .RenderGrouped<VertexPositionNormalTexture>(_texturesShader, vertexBuffer2, indexBuffer2, GraphicsPasses.Textured)
                .SetRenderingConfigs(BlendState.Additive, DepthStencilState.DepthRead, RasterizerStates.Default)
                .ApplyActiveCameraToShader(_starShader)
                .RenderGrouped<VertexPositionNormalTexture>(_starShader, starVertexBuffer, starIndexBuffer, GraphicsPasses.Stars)
                .ApplyActiveCameraToShader(_coloredShader)
                .RenderGrouped<VertexPositionColor>(_coloredShader, coloredStarVertexBuffer, coloredStarIndexBuffer, GraphicsPasses.ColoredStars)
                .SetRenderingConfigs(BlendState.AlphaBlend, DepthStencilState.Default, RasterizerStates.Default)
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .DrawComponents(GraphicsPasses.Debug)
                .EndDraw()
                .Build();
        }
    }
}