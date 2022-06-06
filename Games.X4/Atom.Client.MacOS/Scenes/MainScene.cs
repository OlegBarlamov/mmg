using System;
using Console.Core;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using NetExtensions.Geometry;
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

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;

        private BasicEffect _effect;

        public MainScene(
            MainSceneDataModel model,
            [NotNull] IInputService inputService,
            ICamera3DService camera3DService,
            [NotNull] IRandomService randomService,
            IConsoleController consoleController,
            IDebugInfoService debugInfoService,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection)
            :base(nameof(MainScene))
        {
            DataModel = model;
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));

            Camera3DService.SetActiveCamera(_camera);

            _cameraController = new FirstPersonCameraController(inputService, _camera, DebugInfoService);
        }

        protected override void OnFirstOpening()
        {
            base.OnFirstOpening();
            
            AddView(new Grid3DComponentData
            {
                GraphicsPassName = "Render_Grouped"
            });
            AddView(new DebugInfoComponentData
            {
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
                GraphicsPassName = "debug"
            });
            
            for (int i = 0; i < 10; i++)
            {
                var model = new BoxComponentDataModel
                {
                    Color = ColorsExtensions.GetRandomColor(new Random()),
                    GraphicsPassName = "Render_Grouped",
                    Position = new Vector3(i*2),
                    Scale = new Vector3(1+i)
                };
                AddView(model);
            }

            foreach (var mapCell in DataModel.GalaxiesMap.EnumerateCells())
            {
                var cell = mapCell.Value;
                cell.GalaxiesTree.NodeSubdivided += GalaxiesTreeOnNodeSubdivided;
                var boxModel = BoxComponentDataModel.FromBoundingBox(cell.GalaxiesTree.BoundingBox);
                boxModel.GraphicsPassName = "Render_Grouped";
                AddView(boxModel);
            }

            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<float, float, float>("pos", "Set camera position",
                (x, y, z) =>
                {
                    _camera.Position = new Vector3(x, y, z);
                }));
        }
        
        private void GalaxiesTreeOnNodeSubdivided(AutoSplitOctreeNode<Galaxy> node)
        {
            node.NodeSubdivided -= GalaxiesTreeOnNodeSubdivided;
            foreach (var child in node.Children.Nodes)
            {
                ((AutoSplitOctreeNode<Galaxy>) child).NodeSubdivided += GalaxiesTreeOnNodeSubdivided;
                
                var boxModel = BoxComponentDataModel.FromBoundingBox(child.BoundingBox);
                boxModel.GraphicsPassName = "Render_Grouped";
                boxModel.Color = ColorsExtensions.GetNextColor();
                AddView(boxModel);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);
            }

            if (InputService.Keyboard.KeyPressedOnce(Keys.G))
            {
                var mapCell = DataModel.GalaxiesMap.GetCell(Point3D.Zero);
                var randomPos = RandomService.NextVector3(mapCell.World - new Vector3(mapCell.Size) / 2,
                    mapCell.World + new Vector3(mapCell.Size) / 2);
                var g = new Galaxy(mapCell, randomPos);
                mapCell.GalaxiesTree.AddItem(g);
                AddView(g);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _effect?.Dispose();
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            _effect = new BasicEffect(graphicsPipelineBuilder.GraphicsDevice);
            _effect.VertexColorEnabled = true;

            var vertexBuffer = new VertexBuffer(graphicsPipelineBuilder.GraphicsDevice, VertexPositionColor.VertexDeclaration, 100, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsPipelineBuilder.GraphicsDevice, typeof(int), 200, BufferUsage.WriteOnly);
            
            var vertexBuffer2 = new VertexBuffer(graphicsPipelineBuilder.GraphicsDevice, VertexPositionColor.VertexDeclaration, 100, BufferUsage.WriteOnly);
            var indexBuffer2 = new IndexBuffer(graphicsPipelineBuilder.GraphicsDevice, typeof(int), 200, BufferUsage.WriteOnly);

            return graphicsPipelineBuilder
                .Clear(Color.Black)
                .SetActiveCamera(_effect)
                .SimpleRender<VertexPositionColor>(_effect, vertexBuffer, indexBuffer, "Render")
                .RenderGrouped<VertexPositionColor>(_effect, vertexBuffer2, indexBuffer2,  "Render_Grouped")
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .DrawComponents("debug")
                .EndDraw()
                .Build();
        }
    }
}