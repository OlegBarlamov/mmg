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

        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), new Vector3(9, 10, 10))
        {
            FarPlaneDistance = float.MaxValue
        };
        private readonly FirstPersonCameraController _cameraController;

        private BasicEffect _effect;
        
        private readonly Dictionary<string, IGraphicComponent> _objectsOnGalaxiesScene = new Dictionary<string, IGraphicComponent>();
        private readonly Dictionary<string, IGraphicComponent> _objectsOnStarsScene = new Dictionary<string, IGraphicComponent>();

        public MainScene(
            MainSceneDataModel model,
            [NotNull] IInputService inputService,
            ICamera3DService camera3DService,
            [NotNull] IRandomService randomService,
            IConsoleController consoleController,
            IDebugInfoService debugInfoService,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection,
            [NotNull] IMainUpdatesTasksProcessor mainUpdatesTasksProcessor,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor
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
        private AutoSplitOctreeNode<Star> _cameraStarsNode;
        private CancellationTokenSource _newCellCancellationTokenSource;
        private CancellationTokenSource _newStarsCellCancellationTokenSource;
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
                    _newCellCancellationTokenSource?.Dispose();
                    _newCellCancellationTokenSource = new CancellationTokenSource();
                    
                    foreach (var boxComponent in _objectsOnGalaxiesScene)
                    {
                        var box = boxComponent.Value.BoundingBox.Value;
                        var center = (box.Max + box.Min) / 2;
                        var size = (box.Max - box.Min).Length();
                        if ((_camera.Position - center).Length() > WorldConstants.GalaxiesMapCellSize * 1.5f + size / 2)
                        {
                            MainUpdatesTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                            {
                                RemoveView((IView)boxComponent.Value);
                                _objectsOnGalaxiesScene.Remove(boxComponent.Key);
                                    
                            }, _newCellCancellationTokenSource.Token));
                        }
                    }

                    var mapRec = RectangleBox.FromCenterAndRadius(newCameraPointOnMap.MapPoint, new Point3D(1));
                    foreach (var mapPoint in mapRec.EnumeratePoints())
                    {
                        var mapCell = DataModel.GalaxiesMap.GetCell(mapPoint);
                        if (mapCell == null)
                            continue;

                        foreach (var leaf in mapCell.GalaxiesTree.EnumerateLeafsInRangeAroundPoint(_camera.Position, WorldConstants.GalaxiesMapCellSize * 1.5f))
                        {
                            var galaxies = leaf.Data;
                            foreach (var galaxy in galaxies)
                            {
                                if (!_objectsOnGalaxiesScene.ContainsKey(galaxy.Name))
                                {
                                    MainUpdatesTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                                        {
                                            _objectsOnGalaxiesScene.Add(galaxy.Name, AddView(galaxy));
                                            
                                        }, _newCellCancellationTokenSource.Token));
                                }

                                if (leaf == newGalaxiesNode)
                                {
                                    // the current octree-node
                                    if (galaxy.StarsOctree.Data.Count == 0)
                                    {
                                        BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                                        {
                                            for (int i = 0; i < 100; i++)
                                            {
                                                var position = RandomService.NextVector3(-galaxy.Size / 2, galaxy.Size / 2);
                                                var newStar = new Star(position, galaxy, NamesGenerator.Hash(HashType.SmallGuid, $"{galaxy.Name}_star"));
                                                galaxy.AddStar(newStar);
                                            }
                                        }, CancellationToken.None));
                                    }
                                }  
                            }

                            if (!_objectsOnGalaxiesScene.ContainsKey(leaf.BoundingBox.ToString()))
                            {
                                MainUpdatesTasksProcessor.EnqueueTask(new SimpleDelayedTask(time =>
                                {
                                    var boxModel = BoxComponentDataModel.FromBoundingBox(leaf.BoundingBox);
                                    boxModel.GraphicsPassName = "Render_Grouped";
                                    boxModel.Color = Color.Pink;
                                    var box = new FramedBoxComponent(boxModel);
                                    box.SetName(box.BoundingBox.ToString());
                                    AddView(box);
                                    
                                    _objectsOnGalaxiesScene.Add(box.Name, box);
                                    
                                }, _newCellCancellationTokenSource.Token));
                            }
                        }
                    }
                }
                else
                {
                    var galaxies = newGalaxiesNode.Data;
                    AutoSplitOctreeNode<Star> newStarsNode = null;
                    Galaxy activeGalaxy = null;

                    foreach (var galaxy in galaxies)
                    {
                        if ((galaxy.Position - _camera.Position).Length() < galaxy.Size.X / 2)
                        {
                            // Update stars scene
                            var starsOctree = galaxy.StarsOctree;
                            activeGalaxy = galaxy;
                            newStarsNode = (AutoSplitOctreeNode<Star>)starsOctree.GetLeafWithPoint(_camera.Position - galaxy.Position);
                        }
                    }

                    if (newStarsNode != _cameraStarsNode)
                    {
                        _cameraStarsNode = newStarsNode;

                        if (activeGalaxy != null)
                        {
                            var localPosition = _camera.Position - activeGalaxy.Position;

                            _newStarsCellCancellationTokenSource?.Cancel();
                            _newStarsCellCancellationTokenSource?.Dispose();
                            _newStarsCellCancellationTokenSource = new CancellationTokenSource();

                            foreach (var starsLeaf in newStarsNode.EnumerateLeafsInRangeAroundPoint(localPosition, 50f))
                            {
                                var stars = starsLeaf.Data;
                                var boundingBoxInWorld =
                                    new BoundingBox(starsLeaf.BoundingBox.Min + activeGalaxy.Position,
                                        starsLeaf.BoundingBox.Max + activeGalaxy.Position);

                                if (!_objectsOnStarsScene.ContainsKey(boundingBoxInWorld.ToString()))
                                {
                                    MainUpdatesTasksProcessor.EnqueueTask(
                                        new SimpleDelayedTask(time =>
                                        {
                                            var boxModel = BoxComponentDataModel.FromBoundingBox(boundingBoxInWorld);
                                            boxModel.GraphicsPassName = "Render_Grouped";
                                            boxModel.Color = Color.Orange;
                                            var box = new FramedBoxComponent(boxModel);
                                            box.SetName(boundingBoxInWorld.ToString());
                                            AddView(box);

                                            _objectsOnStarsScene.Add(box.Name, box);
                                        }, _newStarsCellCancellationTokenSource.Token));
                                }

                                // foreach (var star in stars)
                                // {
                                //     
                                // }
                            }
                        }
                    }
                }
                
                
                
                DebugInfoService.SetCounter("my_components", _objectsOnGalaxiesScene.Count);
                
            }
            
            MainUpdatesTasksProcessor.Update(gameTime);
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