using System;
using System.Collections.Generic;
using Atom.Client.MacOS.Components;
using Console.Core;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
{
    public class MainScene : Scene
    {
        private MainSceneDataModel DataModel { get; }
        private ICamera3DService Camera3DService { get; }
        private AstronomicalMapGenerator MapGenerator { get; }
        private IConsoleController ConsoleController { get; }
        private IDebugInfoService DebugInfoService { get; }

        private BasicEffect _effect;
        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;
        private readonly List<StarViewComponent> _starComponents = new List<StarViewComponent>();
        private List<AstronomicalMapCell> _activeCells = new List<AstronomicalMapCell>();

        private RectangleBox _activeCellsRec;
        private AstronomicalMapCell _cameraCell;

        public MainScene(MainSceneDataModel model, [NotNull] ICamera3DService camera3DService, IInputService inputService, IConsoleResourcePackage consoleResourcePackage,
            [NotNull] AstronomicalMapGenerator mapGenerator, FirstPersonCameraProvider firstPersonCameraProvider,
            [NotNull] IConsoleController consoleController, [NotNull] IDebugInfoService debugInfoService)
            :base("MainScene")
        {
            DataModel = model;
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            MapGenerator = mapGenerator ?? throw new ArgumentNullException(nameof(mapGenerator));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));

            Camera3DService.SetActiveCamera(_camera);
            firstPersonCameraProvider.Camera = _camera;
            
            var gridData = new Grid3DComponentData
            {
                GraphicsPassName = "Render_Identical"
            };
            AddView(gridData);
            
            AddView(new DebugInfoComponentData
            {
                Font = consoleResourcePackage.ConsoleFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
                GraphicsPassName = "debug"
            });

            _cameraController = new FirstPersonCameraController(inputService, _camera, DebugInfoService);
        }

        protected override void OnFirstOpening()
        {
            base.OnFirstOpening();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);
            }

            _cameraCell = DataModel.AstronomicalMap.FindPoint(_camera.Position);
            _activeCellsRec = RectangleBox.FromCenterAndRadius(_cameraCell.MapPoint, new Point3D(2));
            DebugInfoService.SetLabel("sector", _cameraCell.MapPoint.ToString());
            DebugInfoService.SetLabel("active_rec", _activeCellsRec.ToString());

            var newActiveCells = new List<AstronomicalMapCell>();
            
            //_activeCells.Clear();
            foreach (var pointAndCell in DataModel.AstronomicalMap.EnumerateCells(_activeCellsRec.Start, _activeCellsRec.End))
            {
                var point = pointAndCell.Item1;
                var cell = pointAndCell.Item2;

                if (cell == null)
                {
                    cell = MapGenerator.GenerateCell(point);
                    DataModel.AstronomicalMap.SetCell(point, cell);
                    foreach (var starModel in cell.Stars)
                    {
                        _starComponents.Add((StarViewComponent) AddView(starModel));
                    }
                }
                else
                {
                    if (!_activeCells.Contains(cell))
                    {
                        foreach (var starModel in cell.Stars)
                        {
                            _starComponents.Add((StarViewComponent) AddView(starModel));
                        }
                    }
                }
                
                newActiveCells.Add(cell);
            }

            _activeCells = newActiveCells;

            var toRemoveComponents = new List<StarViewComponent>();
            foreach (var starComponent in _starComponents)
            {
                if (!_activeCellsRec.Contains(starComponent.Model.MapCell))
                {
                    RemoveView(starComponent);
                    toRemoveComponents.Add(starComponent);
                }
            }

            foreach (var starViewComponent in toRemoveComponents)
            {
                _starComponents.Remove(starViewComponent);
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