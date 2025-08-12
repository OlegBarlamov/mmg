using System;
using System.Collections.Generic;
using Console.Core;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.SceneComponents.Controllers;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NetExtensions.Geometry;

namespace River.Client.MacOS
{
    internal class MainScene : Scene
    {
        private RiverMap Map { get; }
        private IInputService InputService { get; }
        public IDisplayService DisplayService { get; }
        public ICamera2DService Camera2DService { get; }
        public TilesResourcePackage TilesResourcePackage { get; }
        public IConsoleController ConsoleController { get; }

        private readonly Dictionary<MapTile, WaterController> _waterControllers = new Dictionary<MapTile, WaterController>();
        private readonly List<Tuple<MapTile, MapTile>> _delayedMapCellTypeChangedEvents = new List<Tuple<MapTile, MapTile>>();
        private bool _isUpdateInProgress;

        private SimpleCamera2D _camera2D;
        
        public MainScene([NotNull] RiverMap map, [NotNull] IInputService inputService, [NotNull] IDisplayService displayService,
            [NotNull] ICamera2DService camera2DService, [NotNull] TilesResourcePackage tilesResourcePackage,
            [NotNull] IConsoleController consoleController)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
            TilesResourcePackage = tilesResourcePackage ?? throw new ArgumentNullException(nameof(tilesResourcePackage));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));

            Map.MapCellTypeChanged += MapOnMapCellTypeChanged;
        }

        protected override void Initialize()
        {
            AddView(Map);
            
            _camera2D = new SimpleCamera2D(new SizeInt(DisplayService.PreferredBackBufferWidth, DisplayService.PreferredBackBufferHeight),
                new Vector2(Map.Width, Map.Width));
             Camera2DService.SetActiveCamera(_camera2D);
             
            AddController(new KeyboardCamera2DController(InputService, _camera2D,
                new KeyboardCamera2DController.KeysMap()));
            
            Map.ReplaceMapTile(new WaterMapTile(new Point(0, 10), new Vector2(1, 0), 1));
        }

        public override void Dispose()
        {
            Map.MapCellTypeChanged -= MapOnMapCellTypeChanged;
            
            base.Dispose();
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset().Build();
        }

        protected override void Update(GameTime gameTime)
        {
            _isUpdateInProgress = true;

            try
            {
                base.Update(gameTime);

                if (InputService.Keyboard.KeyPressedOnce(Keys.OemTilde))
                {
                    if (ConsoleController.IsShowed)
                        ConsoleController.Hide();
                    else 
                        ConsoleController.Show();
                }

                if (InputService.Keyboard.KeyPressedOnce(Keys.Space))
                {
                    foreach (var waterController in _waterControllers.Values)
                    {
                        waterController.ProcessWaterTick();
                    }
                }

                foreach (var typeChangedEvent in _delayedMapCellTypeChangedEvents)
                {
                    ProcessMapCellTypeChangedEvent(typeChangedEvent.Item1, typeChangedEvent.Item2);
                }
                _delayedMapCellTypeChangedEvents.Clear();
            }
            finally
            {
                _isUpdateInProgress = false;
            }
        }

        private void MapOnMapCellTypeChanged(MapTile oldCell, MapTile newCell)
        {
            if (_isUpdateInProgress)
            {
                _delayedMapCellTypeChangedEvents.Add(new Tuple<MapTile, MapTile>(oldCell, newCell));
            }
            else
            {
                ProcessMapCellTypeChangedEvent(oldCell, newCell);
            }
        }

        private void ProcessMapCellTypeChangedEvent(MapTile oldCell, MapTile newCell)
        {
            if (oldCell.MapTileType == MapTileType.Water)
            {
                RemoveController(oldCell);
                _waterControllers.Remove(oldCell);
            }

            if (newCell.MapTileType == MapTileType.Water)
            {
                var controller = AddController(newCell);
                _waterControllers.Add(newCell, (WaterController)controller);
            }
        }
    }
}