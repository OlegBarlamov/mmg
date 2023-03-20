using System;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace River.Client.MacOS
{
    public enum MapTileType
    {
        Empty,
        Ground,
        Water
    }

    internal class EmptyMapTile : MapTile
    {
        public EmptyMapTile(Point mapPoint) : base(MapTileType.Empty, mapPoint)
        {
        }
    }
    
    internal class GroundMapTile : MapTile
    {
        public GroundMapTile(Point mapPoint) : base(MapTileType.Ground, mapPoint)
        {
        }
    }
    
    public class WaterMapTile : MapTile
    {
        public event Action DataChanged;
        
        public bool Passive { get; set; }
        
        public Vector2 Vector { get; }

        public float Power { get; set; } = 1;
        
        public WaterMapTile(Point mapPoint, Vector2 vector, float power) : base(MapTileType.Water, mapPoint)
        {
            Vector = vector;
            Power = power;
        }
    }

    public abstract class MapTile
    {
        public Point MapPoint { get; }
        public MapTileType MapTileType { get; }

        protected MapTile(MapTileType mapTileType, Point mapPoint)
        {
            MapTileType = mapTileType;
            MapPoint = mapPoint;
        }
    }

    [UsedImplicitly]
    public class WaterView : View<WaterMapTile, WaterController>
    {
        public IDisplayService DisplayService { get; }
        public RiverMap RiverMap { get; }
        
        private readonly DrawVectorComponentDataModel _vectorData;

        public WaterView([NotNull] WaterMapTile model, [NotNull] IDisplayService displayService, [NotNull] RiverMap riverMap, TilesResourcePackage resourcePackage)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            RiverMap = riverMap ?? throw new ArgumentNullException(nameof(riverMap));

            _vectorData = new DrawVectorComponentDataModel(
                new Vector2(
                    DisplayService.PreferredBackBufferWidth / (float)riverMap.Width * (model.MapPoint.X + 0.5f),
                    DisplayService.PreferredBackBufferHeight -
                    DisplayService.PreferredBackBufferHeight / (float)riverMap.Height * (model.MapPoint.Y - 0.5f)),
                model.Vector * 10f)
            {
                Texture = resourcePackage.ArrowsTexture
            };
            
            model.DataChanged += ModelOnDataChanged;
        }

        protected override void OnDestroy()
        {
            DataModel.DataChanged -= ModelOnDataChanged;
            
            base.OnDestroy();
        }

        private void ModelOnDataChanged()
        {
            _vectorData.Vector = DataModel.Vector * 10f;
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            scene.AddView(_vectorData);
        }

        protected override void OnDetached(SceneBase scene)
        {
            scene.RemoveView(_vectorData);

            base.OnDetached(scene);
        }
    }

    [UsedImplicitly]
    public class WaterController : Controller<WaterMapTile>
    {
        public WaterMapTile WaterMapTile { get; }
        public RiverMap Map { get; }

        public WaterController([NotNull] WaterMapTile waterMapTile, [NotNull] RiverMap map)
        {
            WaterMapTile = waterMapTile ?? throw new ArgumentNullException(nameof(waterMapTile));
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public void ProcessWaterTick()
        {
            if (WaterMapTile.Passive)
                return;
            
            var point = WaterMapTile.MapPoint;
            var grounded = Map.Map[point.X, point.Y - 1].MapTileType != MapTileType.Empty; 
            if (WaterMapTile.Vector == Vector2.UnitX)
            {
                if (grounded)
                {
                    var forward = Map.Map[point.X + 1, point.Y];
                    if (forward.MapTileType == MapTileType.Empty)
                    {
                        Map.ReplaceMapTile(new WaterMapTile(forward.MapPoint, Vector2.UnitX, WaterMapTile.Power));
                    }
                    else if (forward.MapTileType == MapTileType.Ground)
                    {
                        
                    }
                }
                else
                {
                
                }
            }

            WaterMapTile.Passive = true;
        }
    }
    
    public class RiverMap : ViewModel
    {
        public event Action<MapTile, MapTile> MapCellTypeChanged;
        
        public int Width => Map.GetLength(0);
        public int Height => Map.GetLength(1);
        public MapTile[,] Map { get; }

        public RiverMap([NotNull] MapTile[,] map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public void ReplaceMapTile([NotNull] MapTile mapTile)
        {
            if (mapTile == null) throw new ArgumentNullException(nameof(mapTile));
            
            var point = mapTile.MapPoint;
            var oldPoint = Map[point.X, point.Y];
            Map[point.X, point.Y] = mapTile;
            
            MapCellTypeChanged?.Invoke(oldPoint, mapTile);
        }

        public static RiverMap Generate(int width, int height)
        {
            var map = new MapTile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y < 10)
                    {
                        map[x, y] = new GroundMapTile(new Point(x, y));
                    }
                    else
                    {
                        map[x, y] = new EmptyMapTile(new Point(x, y));
                    }
                }
            }

            return new RiverMap(map);
        }
    }
}