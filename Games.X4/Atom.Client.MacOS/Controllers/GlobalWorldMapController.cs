using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Controllers
{
    class GlobalWorldMapController : IGlobalWorldMapController 
    {
        public event Action<WorldMapCellContent> CellRevealed;
        public event Action<WorldMapCellContent> CellHidden;
        public event Action<WorldMapCellContent> CellUnwrapped;
        public event Action<WorldMapCellContent> CellWrapped;

        private GlobalWorldMap GlobalWorldMap { get; }

        private const int RevealRadius = 3;

        private Vector3? _lastPlayerPosition;
        private Point3D? _lastMapCell;
        private readonly List<GlobalWorldMapCell> _revealedCells = new List<GlobalWorldMapCell>();
        private readonly List<GlobalWorldMapCell> _unwrappedCells = new List<GlobalWorldMapCell>();

        public GlobalWorldMapController([NotNull] GlobalWorldMap globalWorldMap)
        {
            GlobalWorldMap = globalWorldMap ?? throw new ArgumentNullException(nameof(globalWorldMap));
        }

        public void Update(Vector3 playerPosition, GameTime gameTime)
        {
            if (_lastPlayerPosition == playerPosition)
                return;
            
            var mapCell = GlobalWorldMap.FindPoint(playerPosition);
            if (_lastMapCell != mapCell.MapPoint)
            {
                _lastMapCell = mapCell.MapPoint;
                var minPoint = mapCell.MapPoint - new Point3D(RevealRadius);
                var maxPoint = mapCell.MapPoint + new Point3D(RevealRadius);

                WrapCellsOutOfDistance(playerPosition);
                HideCellsOutOfRange(minPoint, maxPoint);
                RevealNewCellsInRadius(mapCell.MapPoint, RevealRadius);
            }
            else
            {
                _lastPlayerPosition = playerPosition;
                UnwrapCellsInDistance(playerPosition);
            }
        }

        private void UnwrapCellsInDistance(Vector3 playerPosition)
        {
            foreach (var worldMapCell in _revealedCells)
            {
                var worldPosition = worldMapCell.Content.GetWorldPosition();
                if ((playerPosition - worldPosition).Length() < worldMapCell.Content.DistanceToUnwrapDetails)
                {
                    if (!_unwrappedCells.Contains(worldMapCell))
                    {
                        _unwrappedCells.Add(worldMapCell);
                        CellUnwrapped?.Invoke(worldMapCell.Content);
                    }
                }
            }
        }

        private void WrapCellsOutOfDistance(Vector3 playerPosition)
        {
            for (int i = 0; i < _unwrappedCells.Count; i++)
            {
                var unwrappedCell = _unwrappedCells[i];
                var worldPosition = unwrappedCell.Content.GetWorldPosition();
                if ((playerPosition - worldPosition).Length() > unwrappedCell.Content.DistanceToUnwrapDetails)
                {
                    _unwrappedCells.RemoveAt(i);
                    i--;
                    CellWrapped?.Invoke(unwrappedCell.Content);
                }
            }
        }

        private void RevealNewCellsInRadius(Point3D center, int radius)
        {
            var points = GlobalWorldMap.EnumerateCells(center, radius);
            foreach (var keyValuePair in points)
            {
                var cell = keyValuePair.Value;
                if (!_revealedCells.Contains(cell))
                {
                    _revealedCells.Add(cell);
                    CellRevealed?.Invoke(cell.Content);
                }
            }
        }

        private void HideCellsOutOfRange(Point3D minPoint, Point3D maxPoint)
        {
            for (int i = 0; i < _revealedCells.Count; i++)
            {
                var potentialHiddenCell = _revealedCells[i];
                if (!potentialHiddenCell.MapPoint.InRange(minPoint, maxPoint))
                {
                    _revealedCells.RemoveAt(i);
                    i--;
                    CellHidden?.Invoke(potentialHiddenCell.Content);
                }
            }
        }
    }
}