using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using NetExtensions.Geometry;
using X4World;
using X4World.Generation;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Services.Implementations
{
    public class DefaultMapUpdater : IMapUpdater
    {
        [NotNull] private GalaxiesMap Map { get; }
        [NotNull] private IGalaxiesMapGenerator GalaxiesMapGenerator { get; }
        public IStarsMapGenerator StarsMapGenerator { get; }
        [NotNull] private IDebugInfoService DebugInfoService { get; }
        [NotNull] private DirectionalCamera3D ActiveCamera { get; }

        private Point3D _lastCameraMapPoint = Point3D.Zero;
        
        private readonly List<GalaxiesMapCell> _cellsPendingToBeAdded = new List<GalaxiesMapCell>();
        private readonly List<GalaxiesMapCell> _cellsPendingToBeRemoved = new List<GalaxiesMapCell>(); 
        private readonly List<GalaxiesMapCell> _tempArray = new List<GalaxiesMapCell>();

        private readonly List<Star> _starsPendingToBeAdded = new List<Star>();
        private readonly List<Star> _starsPendingToBeRemoved = new List<Star>();
            
        public DefaultMapUpdater(
            [NotNull] GalaxiesMap map,
            [NotNull] IGalaxiesMapGenerator galaxiesMapGenerator,
            [NotNull] IStarsMapGenerator starsMapGenerator,
            [NotNull] IDebugInfoService debugInfoService,
            [NotNull] DirectionalCamera3D activeCamera)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            GalaxiesMapGenerator = galaxiesMapGenerator ?? throw new ArgumentNullException(nameof(galaxiesMapGenerator));
            StarsMapGenerator = starsMapGenerator ?? throw new ArgumentNullException(nameof(starsMapGenerator));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            ActiveCamera = activeCamera ?? throw new ArgumentNullException(nameof(activeCamera));
        }

        public MapUpdateResult Update(GameTime gameTime)
        {
            var cameraPosition = ActiveCamera.Position;
            var mapPoint = Map.FindPoint(cameraPosition);
            
            DebugInfoService.SetLabel("sector", mapPoint.MapPoint.ToString());
            
            if (mapPoint.MapPoint != _lastCameraMapPoint)
            {
                var newRec = RectangleBox.FromCenterAndRadius(mapPoint.MapPoint,
                    new Point3D(MainSceneDataModel.AstronomicMapViewRadius));
                var oldRec = RectangleBox.FromCenterAndRadius(_lastCameraMapPoint,
                    new Point3D(MainSceneDataModel.AstronomicMapViewRadius));
                
                DebugInfoService.SetLabel("active_rec", newRec.ToString());
                
                var intersect = RectangleBox.Intersect(oldRec, newRec);
                
                _tempArray.Clear();
                foreach (var mapCell in _cellsPendingToBeAdded)
                {
                    if (!newRec.Contains(mapCell.MapPoint))
                        _tempArray.Add(mapCell);
                }
                _cellsPendingToBeAdded.RemoveRange(_tempArray);

                _tempArray.Clear();
                foreach (var mapCell in _cellsPendingToBeRemoved)
                {
                    if (newRec.Contains(mapCell.MapPoint))
                        _tempArray.Add(mapCell);
                }
                _cellsPendingToBeAdded.RemoveRange(_tempArray);

                foreach (var point in oldRec.EnumeratePoints())
                {
                    if (!intersect.Contains(point))
                        _cellsPendingToBeRemoved.Add(Map.GetCell(point));
                }
                
                foreach (var point in newRec.EnumeratePoints())
                {
                    if (!intersect.Contains(point))
                    {
                        var cell = Map.GetCell(point);
                        if (cell == null)
                        {
                            cell = GalaxiesMapGenerator.GenerateCell(point);
                            Map.SetCell(point, cell);
                        }
                        _cellsPendingToBeAdded.Add(cell);
                    }
                }

                foreach (var galaxy in mapPoint.Galaxies)
                {
                    if (galaxy.Stars.Count == 0) 
                        StarsMapGenerator.GenerateStarsForGalaxy(galaxy);
                    
                    _starsPendingToBeAdded.AddRange(galaxy.Stars);
                }
                
                var lastPoint = Map.GetCell(_lastCameraMapPoint);
                foreach (var galaxy in lastPoint.Galaxies)
                {
                    _starsPendingToBeRemoved.AddRange(galaxy.Stars);
                }
            }

            _lastCameraMapPoint = mapPoint.MapPoint;

            var addedMapPoints = GetFirstElements(_cellsPendingToBeAdded, 1).ToArray();
            var removedMapPoints = GetFirstElements(_cellsPendingToBeRemoved, 1).ToArray();
            var addedStars = GetFirstElements(_starsPendingToBeAdded, 5).ToArray();
            var removedStars = GetFirstElements(_starsPendingToBeRemoved, 5).ToArray();
            
            var result = new MapUpdateResult(addedMapPoints, removedMapPoints, addedStars, removedStars);
            
            _cellsPendingToBeAdded.RemoveRange(addedMapPoints);
            _cellsPendingToBeRemoved.RemoveRange(removedMapPoints);
            _starsPendingToBeAdded.RemoveRange(addedStars);
            _starsPendingToBeRemoved.RemoveRange(removedStars);

            return result;
        }

        private IEnumerable<T> GetFirstElements<T>(IReadOnlyList<T> array, int maxCountToPickup)
        {
            for (int i = 0; i < Math.Min(maxCountToPickup, array.Count); i++)
            {
                yield return array[i];
            }
        }
    }
}