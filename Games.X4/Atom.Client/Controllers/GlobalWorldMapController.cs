using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.Controllers
{
    class GlobalWorldMapController : IGlobalWorldMapController 
    {
        public event Action<WorldMapCellContent> CellRevealed;
        public event Action<WorldMapCellContent> CellHidden;

        private GlobalWorldMap GlobalWorldMap { get; }
        public IDebugInfoService DebugInfoService { get; }

        private const int RevealRadius = 3;

        private Vector3? _lastPlayerPosition;
        private Point3D? _lastMapCell;
        private readonly List<GlobalWorldMapCell> _revealedCells = new List<GlobalWorldMapCell>();

        public GlobalWorldMapController([NotNull] GlobalWorldMap globalWorldMap, [NotNull] IDebugInfoService debugInfoService)
        {
            GlobalWorldMap = globalWorldMap ?? throw new ArgumentNullException(nameof(globalWorldMap));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
        }

        public void Update(Vector3 playerPosition, GameTime gameTime)
        {
            if (_lastPlayerPosition == playerPosition)
                return;
            
            var mapCell = GlobalWorldMap.FindPoint(playerPosition);
            if (_lastMapCell != mapCell.MapPoint)
            {
                DebugInfoService.SetLabel("map_pos", mapCell.MapPoint.ToString());
                
                _lastMapCell = mapCell.MapPoint;
                var minPoint = mapCell.MapPoint - new Point3D(RevealRadius);
                var maxPoint = mapCell.MapPoint + new Point3D(RevealRadius);

                HideCellsOutOfRange(minPoint, maxPoint);
                RevealNewCellsInRadius(mapCell.MapPoint, RevealRadius);
            }
            else
            {
                _lastPlayerPosition = playerPosition;
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