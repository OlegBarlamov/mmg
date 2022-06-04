using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS.Services.Implementations
{
    public class DefaultAstronomicalMapUpdater : IAstronomicalMapUpdater
    {
        [NotNull] private AstronomicalMap Map { get; }
        [NotNull] private IAstronomicMapGenerator AstronomicMapGenerator { get; }
        [NotNull] private IDebugInfoService DebugInfoService { get; }
        [NotNull] private DirectionalCamera3D ActiveCamera { get; }

        private Point3D _lastCameraMapPoint = Point3D.Zero;
        
        private readonly List<AstronomicalMapCell> _cellsPendingToBeAdded = new List<AstronomicalMapCell>();
        private readonly List<AstronomicalMapCell> _cellsPendingToBeRemoved = new List<AstronomicalMapCell>(); 
        private readonly List<AstronomicalMapCell> _tempArray = new List<AstronomicalMapCell>();
            
        public DefaultAstronomicalMapUpdater(
            [NotNull] AstronomicalMap map,
            [NotNull] IAstronomicMapGenerator astronomicMapGenerator,
            [NotNull] IDebugInfoService debugInfoService,
            [NotNull] DirectionalCamera3D activeCamera)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            AstronomicMapGenerator = astronomicMapGenerator ?? throw new ArgumentNullException(nameof(astronomicMapGenerator));
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
                            cell = AstronomicMapGenerator.GenerateCell(point);
                            Map.SetCell(point, cell);
                        }
                        _cellsPendingToBeAdded.Add(cell);
                    }
                }
            }

            _lastCameraMapPoint = mapPoint.MapPoint;
            
            var result = new MapUpdateResult(
                GetFirstElements(_cellsPendingToBeAdded, 1).ToArray(),
                GetFirstElements(_cellsPendingToBeRemoved, 1).ToArray());
            
            _cellsPendingToBeAdded.RemoveRange(result.AddedPoints);
            _cellsPendingToBeRemoved.RemoveRange(result.RemovedPoints);

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