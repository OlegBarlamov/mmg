using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

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

    public abstract class MapTile : IGridCell<Point>
    {
        public Point MapPoint { get; }
        public MapTileType MapTileType { get; }

        protected MapTile(MapTileType mapTileType, Point mapPoint)
        {
            MapTileType = mapTileType;
            MapPoint = mapPoint;
        }

        public Point GetPointOnMap()
        {
            return MapPoint;
        }
    }

    [UsedImplicitly]
    public class WaterView : View<WaterMapTile, WaterController>
    {
        public IDisplayService DisplayService { get; }
        public RiverMap RiverMap { get; }
        
        private readonly DrawVectorComponent _vectorComponent;
        private readonly DrawVectorComponentDataModel _vectorComponentData;

        public WaterView([NotNull] WaterMapTile model, [NotNull] IDisplayService displayService, [NotNull] RiverMap riverMap, TilesResourcePackage resourcePackage)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            RiverMap = riverMap ?? throw new ArgumentNullException(nameof(riverMap));

            _vectorComponentData = new DrawVectorComponentDataModel(
                new Vector2(model.MapPoint.X + 0.5f,
                    model.MapPoint.Y - 0.5f),
                model.Vector * 10f)
            {
                Texture = resourcePackage.ArrowsTexture
            };
            _vectorComponent = new DrawVectorComponent(_vectorComponentData);
            
            model.DataChanged += ModelOnDataChanged;
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            
            _vectorComponent.Draw(gameTime, context);
        }

        protected override void OnDestroy()
        {
            DataModel.DataChanged -= ModelOnDataChanged;
            
            base.OnDestroy();
        }

        private void ModelOnDataChanged()
        {
            _vectorComponentData.Vector = DataModel.Vector * 10f;
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
            var grounded = Map[point.X, point.Y - 1].MapTileType != MapTileType.Empty; 
            if (WaterMapTile.Vector == Vector2.UnitX)
            {
                if (grounded)
                {
                    var forward = Map[point.X + 1, point.Y];
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
        
        public MapTile this[int x, int y] => Map.GetCell(x, y);

        public int Width => Map.GetUpperBound().X;
        public int Height => Map.GetUpperBound().Y;
        public IBoundedGrid<Point, MapTile> Map { get; }

        public RiverMap([NotNull] IBoundedGrid<Point, MapTile> data)
        {
            Map = data;
        }

        public void ReplaceMapTile([NotNull] MapTile mapTile)
        {
            if (mapTile == null) throw new ArgumentNullException(nameof(mapTile));
            
            var point = mapTile.MapPoint;
            var oldPoint = Map.GetCell(new Point(point.X, point.Y));
            Map.SetCell(new Point(point.X, point.Y), mapTile);
            
            MapCellTypeChanged?.Invoke(oldPoint, mapTile);
        }

        public static RiverMap Generate(int width, int height)
        {
            var map = new DictionaryBasedGrid<Point, MapTile>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y < 10)
                    {
                        map.SetCell(new Point(x, y), new GroundMapTile(new Point(x, y)));
                    }
                    else
                    {
                         map.SetCell(new Point(x, y), new EmptyMapTile(new Point(x, y)));
                    }
                }
            }

            return new RiverMap(new BoundedGridWrapper<Point, MapTile>(map, Point.Zero, new Point(width, height), GridExtensions.ContainsPoint));
        }
    }
}