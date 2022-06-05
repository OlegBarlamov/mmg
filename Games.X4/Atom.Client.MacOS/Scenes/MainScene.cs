using System;
using System.Collections.Generic;
using Atom.Client.MacOS.Services;
using Atom.Client.MacOS.Services.Implementations;
using Console.Core;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Generation;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Scenes
{
    public class MainScene : Scene
    {
        private MainSceneDataModel DataModel { get; }
        private ICamera3DService Camera3DService { get; }
        private IConsoleController ConsoleController { get; }
        private IDebugInfoService DebugInfoService { get; }
        private IGalaxiesMapGenerator MapGenerator { get; }
        public IStarsMapGenerator StarsMapGenerator { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;

        private BasicEffect _effect;
        private IMapUpdater _mapUpdater;
        
        public MainScene(
            MainSceneDataModel model,
            IInputService inputService,
            ICamera3DService camera3DService,
            IConsoleController consoleController,
            IDebugInfoService debugInfoService,
            [NotNull] IGalaxiesMapGenerator mapGenerator,
            [NotNull] IStarsMapGenerator starsMapGenerator,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection)
            :base(nameof(MainScene))
        {
            DataModel = model;
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            MapGenerator = mapGenerator ?? throw new ArgumentNullException(nameof(mapGenerator));
            StarsMapGenerator = starsMapGenerator ?? throw new ArgumentNullException(nameof(starsMapGenerator));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));

            Camera3DService.SetActiveCamera(_camera);

            _cameraController = new FirstPersonCameraController(inputService, _camera, DebugInfoService);
        }

        protected override void OnFirstOpening()
        {
            base.OnFirstOpening();
            
            AddView(new Grid3DComponentData
            {
                GraphicsPassName = "Render_Identical"
            });
            AddView(new DebugInfoComponentData
            {
                Font = DataModel.MainResourcePackage.DebugInfoFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
                GraphicsPassName = "debug"
            });

            foreach (var mapPoint in DataModel.GalaxiesMap.EnumerateCells())
            {
                var cell = mapPoint.Item2;
                AddMapPointToScene(cell);
                if (mapPoint.Item2.ContainsPoint(_camera.Position))
                {
                    foreach (var galaxy in mapPoint.Item2.Galaxies)
                    {
                        AddStarsToScene(galaxy.Stars);
                    }
                }
            }
            
            _mapUpdater = new DefaultMapUpdater(DataModel.GalaxiesMap, MapGenerator, StarsMapGenerator, DebugInfoService, _camera);
            
            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<float, float, float>("pos", "Set camera position",
                (x, y, z) =>
                {
                    _camera.Position = new Vector3(x, y, z);
                }));
        }

        private void RemoveMapPointFromScene(GalaxiesMapCell cell)
        {
            foreach (var galaxy in cell.Galaxies)
            {
                RemoveView(galaxy);
            }
        }

        private void AddMapPointToScene(GalaxiesMapCell cell)
        {
            foreach (var galaxy in cell.Galaxies)
            {
                AddView(galaxy);
            }
        }

        private void AddStarsToScene(IEnumerable<Star> stars)
        {
            foreach (var star in stars)
            {
                AddView(star);
            }
        }
        
        private void RemoveStarsToScene(IEnumerable<Star> stars)
        {
            foreach (var star in stars)
            {
                RemoveView(star);
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);
            }

            var mapUpdates = _mapUpdater.Update(gameTime);

            foreach (var mapUpdatesAddedPoint in mapUpdates.AddedPoints)
            {
                AddMapPointToScene(mapUpdatesAddedPoint);
            }
            foreach (var mapUpdatesRemovedPoint in mapUpdates.RemovedPoints)
            {
                RemoveMapPointFromScene(mapUpdatesRemovedPoint);
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
                .RenderIdentical<VertexPositionColor>(_effect, vertexBuffer2, indexBuffer2,  "Render_Identical")
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .DrawComponents("debug")
                .EndDraw()
                .Build();
        }
    }
}