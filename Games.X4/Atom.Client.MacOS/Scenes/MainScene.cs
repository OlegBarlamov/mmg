using System;
using System.Collections.Generic;
using System.Threading;
using Atom.Client.MacOS.Components;
using Console.Core;
using Console.Core.Commands;
using Console.FrameworkAdapter;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public ITicksTasksProcessor TicksTasksProcessor { get; }

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;

        private BasicEffect _effect;
        
        private readonly Dictionary<string, IGraphicComponent> _objectsOnScene = new Dictionary<string, IGraphicComponent>();

        public MainScene(
            MainSceneDataModel model,
            [NotNull] IInputService inputService,
            ICamera3DService camera3DService,
            [NotNull] IRandomService randomService,
            IConsoleController consoleController,
            IDebugInfoService debugInfoService,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection,
            [NotNull] ITicksTasksProcessor ticksTasksProcessor
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
            TicksTasksProcessor = ticksTasksProcessor ?? throw new ArgumentNullException(nameof(ticksTasksProcessor));

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

            ExecutableCommandsCollection.AddCommand(new FixedTypedExecutableConsoleCommandDelegate<float, float, float>("pos", "Set camera position",
                (x, y, z) =>
                {
                    _camera.Position = new Vector3(x, y, z);
                }));
        }
        
        private AutoSplitOctreeNode<Galaxy> _cameraGalaxiesNode;
        private CancellationTokenSource _newCellCancellationTokenSource;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ConsoleController.IsShowed)
            {
                _cameraController.Update(gameTime);

                var newCameraPointOnMap = DataModel.GalaxiesMap.FindPoint(_camera.Position);
                var newGalaxiesNode = (AutoSplitOctreeNode<Galaxy>)newCameraPointOnMap.GalaxiesTree.GetLeafWithPoint(_camera.Position);
                
                if (newGalaxiesNode != _cameraGalaxiesNode)
                {
                    _cameraGalaxiesNode = newGalaxiesNode;
                    
                    _newCellCancellationTokenSource?.Cancel();
                    _newCellCancellationTokenSource = new CancellationTokenSource();
                    
                    foreach (var boxComponent in _objectsOnScene)
                    {
                        var box = boxComponent.Value.BoundingBox.Value;
                        var center = (box.Max + box.Min) / 2;
                        var size = (box.Max - box.Min).Length();
                        if ((_camera.Position - center).Length() > 1500f + size / 2)
                        {
                            TicksTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                            {
                                RemoveView((IView)boxComponent.Value);
                                _objectsOnScene.Remove(boxComponent.Key);
                                    
                            }, _newCellCancellationTokenSource.Token));
                        }
                    }

                    var mapRec = RectangleBox.FromCenterAndRadius(newCameraPointOnMap.MapPoint, new Point3D(1));
                    foreach (var mapPoint in mapRec.EnumeratePoints())
                    {
                        var mapCell = DataModel.GalaxiesMap.GetCell(mapPoint);
                        if (mapCell == null)
                            continue;

                        foreach (var leaf in mapCell.GalaxiesTree.EnumerateLeafsInRangeAroundPoint(_camera.Position, 1500f))
                        {
                            var galaxies = leaf.Data;
                            foreach (var galaxy in galaxies)
                            {
                                if (!_objectsOnScene.ContainsKey(galaxy.Name))
                                {
                                    _objectsOnScene.Add(galaxy.Name, AddView(galaxy));
                                }
                            }
                            
                            
                            if (!_objectsOnScene.ContainsKey(leaf.BoundingBox.ToString()))
                            {
                                TicksTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                                {
                                    var boxModel = BoxComponentDataModel.FromBoundingBox(leaf.BoundingBox);
                                    boxModel.GraphicsPassName = "Render_Grouped";
                                    boxModel.Color = Color.Pink;
                                    var box = new FramedBoxComponent(boxModel);
                                    box.SetName(box.BoundingBox.ToString());
                                    AddView(box);
                                    
                                    _objectsOnScene.Add(box.Name, box);
                                    
                                }, _newCellCancellationTokenSource.Token));
                            }
                        }
                    }
                }
                
                DebugInfoService.SetCounter("my_components", _objectsOnScene.Count);
                
            }
            
            TicksTasksProcessor.Update(gameTime);
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