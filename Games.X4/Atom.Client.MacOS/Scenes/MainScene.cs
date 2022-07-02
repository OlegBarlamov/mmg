using System;
using Atom.Client.MacOS.Controllers;
using Atom.Client.MacOS.Services;
using Console.Core;
using Console.Core.Commands;
using Console.FrameworkAdapter;
using FrameworkSDK.Common;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
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

namespace Atom.Client.MacOS.Scenes
{
    public class MainScene : Scene
    {
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

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;
        private readonly IGlobalWorldMapController _globalWorldMapController;
        private readonly IWrappedObjectsController _wrappedObjectsController;

        private BasicEffect _texturesShader;
        private BasicEffect _coloredShader;

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
            [NotNull] IDetailsGeneratorProvider detailsGeneratorProvider
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

            Camera3DService.SetActiveCamera(_camera);

            _cameraController = new FirstPersonCameraController(inputService, _camera, DebugInfoService);
            _globalWorldMapController = new GlobalWorldMapController(DataModel.GlobalWorldMap);
            _globalWorldMapController.CellRevealed += GlobalWorldMapControllerOnCellRevealed;
            _globalWorldMapController.CellHidden += GlobalWorldMapControllerOnCellHidden;
            _globalWorldMapController.CellUnwrapped += GlobalWorldMapControllerOnCellUnwrapped;
            _globalWorldMapController.CellWrapped += GlobalWorldMapControllerOnCellWrapped;
            _wrappedObjectsController = new WrappedObjectsController(DetailsGeneratorProvider);
            _wrappedObjectsController.ObjectRevealed += WrappedObjectsControllerOnObjectRevealed; 
            _wrappedObjectsController.ObjectHidden += WrappedObjectsControllerOnObjectHidden; 
        }

        private void WrappedObjectsControllerOnObjectHidden(IWrappedDetails obj)
        {
            try
            {
                RemoveView(obj);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, "m", FrameworkLogLevel.Error);
            }
        }

        private void WrappedObjectsControllerOnObjectRevealed(IWrappedDetails obj)
        {
            try
            {
                AddView(obj);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, "m", FrameworkLogLevel.Error);
            }
        }

        private void GlobalWorldMapControllerOnCellWrapped(WorldMapCellContent cell)
        {
            _wrappedObjectsController.AddWrappedObject(cell);
            _wrappedObjectsController.RemoveUnwrappedObject(cell);
        }

        private void GlobalWorldMapControllerOnCellUnwrapped(WorldMapCellContent cell)
        {
            _wrappedObjectsController.AddUnwrappedObject(cell);
            _wrappedObjectsController.RemoveWrappedObject(cell);
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
            // BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
            // {
            //     
            // }, CancellationToken.None));
        }

        protected override void OnFirstOpening()
        {
            base.OnFirstOpening();
            
            AddView(new Grid3DComponentView<FunctionController<Grid3DComponentData>>(new Grid3DComponentData
            {
                GraphicsPassName = "Render_Grouped"
            }));
            
            AddView(new DebugInfoComponentData
            {
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
                GraphicsPassName = "debug"
            });

            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<float, float, float>("pos", "Set camera position",
                (x, y, z) =>
                {
                    _camera.Position = new Vector3(x, y, z);
                }));
        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);
            }
            
            _globalWorldMapController.Update(_camera.Position, gameTime);
            _wrappedObjectsController.Update(_camera.Position, gameTime);

            MainUpdatesTasksProcessor.Update(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _texturesShader?.Dispose();
            _coloredShader?.Dispose();
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

            var vertexBuffer = graphicsPipelineBuilder.CreateVertexBugger(VertexPositionColor.VertexDeclaration, 100);
            var indexBuffer = graphicsPipelineBuilder.CreateIndexBuffer(200);
            
            var vertexBuffer2 = graphicsPipelineBuilder.CreateVertexBugger(VertexPositionNormalTexture.VertexDeclaration, 1000);
            var indexBuffer2 = graphicsPipelineBuilder.CreateIndexBuffer(5000);

            return graphicsPipelineBuilder
                .Clear(Color.Black)
                .SetRenderingConfigs(BlendState.Opaque, DepthStencilState.Default, RasterizerStates.Default)
                .SetActiveCamera(_coloredShader)
                //.SimpleRender<VertexPositionColor>(_effect, vertexBuffer2, indexBuffer2, "Render")
                .RenderGrouped<VertexPositionColor>(_coloredShader, vertexBuffer, indexBuffer,  "Render_Grouped")
                .SetActiveCamera(_texturesShader)
                .RenderGrouped<VertexPositionNormalTexture>(_texturesShader, vertexBuffer2, indexBuffer2,  "Render_Textured")
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .DrawComponents("debug")
                .EndDraw()
                .Build();
        }
    }
}