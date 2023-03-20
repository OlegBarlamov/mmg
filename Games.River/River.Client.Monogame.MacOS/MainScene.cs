using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace River.Client.MacOS
{
    internal class MainScene : Scene
    {
        private RiverMap Map { get; }
        private IInputService InputService { get; }

        private readonly Dictionary<MapTile, WaterController> _waterControllers = new Dictionary<MapTile, WaterController>();
        private readonly List<Tuple<MapTile, MapTile>> _delayedMapCellTypeChangedEvents = new List<Tuple<MapTile, MapTile>>();
        private bool _isUpdateInProgress;

        public MainScene([NotNull] RiverMap map, [NotNull] IInputService inputService)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));

            AddView(map);
            
            Map.MapCellTypeChanged += MapOnMapCellTypeChanged;
        }

        protected override void OnFirstOpening()
        {
            base.OnFirstOpening();
            
            Map.ReplaceMapTile(new WaterMapTile(new Point(0, 10), new Vector2(1, 0), 1));
            Map.ReplaceMapTile(new GroundMapTile(new Point(3, 10)));
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