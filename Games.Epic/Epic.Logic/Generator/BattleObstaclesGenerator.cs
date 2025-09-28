using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Logic;
using Epic.Core.Services.BattleObstacles;
using Epic.Core.Services.Battles;
using Epic.Data.BattleObstacles;
using Epic.Data.UnitTypes.Subtypes;
using Epic.Logic.Battle.Map;
using JetBrains.Annotations;

namespace Epic.Logic.Battle.Obstacles
{
    [UsedImplicitly]
    public class BattleObstaclesGenerator : IBattleObstaclesGenerator
    {
        public IBattleObstaclesService BattleObstaclesService { get; }
        
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public BattleObstaclesGenerator([NotNull] IBattleObstaclesService battleObstaclesService)
        {
            BattleObstaclesService = battleObstaclesService ?? throw new ArgumentNullException(nameof(battleObstaclesService));
        }
        
        public async Task<IBattleObstacleObject[]> GenerateForBattle(IBattleObject battleObject)
        {
            var mapSize = battleObject.Width * battleObject.Height;
            var maxStage = 4;
            if (mapSize < 20)
                maxStage = 2;
            if (mapSize < 50)
                maxStage = 3;
            
            int stage = _random.Next(1, maxStage + 1);

            var minSmallObstaclesCount = 0;
            var maxSmallObstaclesCount = 0;
            var minMiddleObstaclesCount = 0;
            var maxMiddleObstaclesCount = 0;
            var minLargeObstaclesCount = 0;
            var maxLargeObstaclesCount = 0;
            switch (stage)
            {
                case 1:
                    break;
                case 2:
                    minSmallObstaclesCount = 1;
                    maxSmallObstaclesCount = 2;
                    break;
                case 3:
                    minSmallObstaclesCount = 0;
                    maxSmallObstaclesCount = 3;
                    minMiddleObstaclesCount = 1;
                    maxMiddleObstaclesCount = 2;
                    break;
                case 4:
                    minSmallObstaclesCount = 0;
                    maxSmallObstaclesCount = 5;
                    minMiddleObstaclesCount = 0;
                    maxMiddleObstaclesCount = 4;
                    minLargeObstaclesCount = 1;
                    maxLargeObstaclesCount = 2;
                    break;;
            }

            var smallObstacleMinWidth = 1;
            var smallObstacleMaxWidth = 2;
            var smallObstacleMinHeight = 1;
            var smallObstacleMaxHeight = 2;
            
            var middleObstacleMinWidth = 3;
            var middleObstacleMaxWidth = 5;
            var middleObstacleMinHeight = 2;
            var middleObstacleMaxHeight = 4;
            
            var largeObstacleMinWidth = 6;
            var largeObstacleMaxWidth = 12;
            var largeObstacleMinHeight = 2;
            var largeObstacleMaxHeight = 5;
            
            var closeToArmy = _random.Next(100) < 20;
            var minColumn = closeToArmy ? 1 : 2;
            var minRow = 0;

            var fields = new List<IBattleObstacleFields>();
            
            for (int i = 0; i < _random.Next(minSmallObstaclesCount, maxSmallObstaclesCount + 1); i++)
            {
                var maxWidth = Math.Min(closeToArmy ? battleObject.Width - 2 : battleObject.Width - 4, smallObstacleMaxWidth);
                var maxHeight = Math.Min(battleObject.Height - 1, smallObstacleMaxHeight);
                
                var width = _random.Next(smallObstacleMinWidth, maxWidth + 1);
                var height = _random.Next(smallObstacleMinHeight, maxHeight + 1);

                var maxColumn = closeToArmy ? battleObject.Width - width - 1 : battleObject.Width - width - 2;
                var maxRow = battleObject.Height - height;
                
                var column = _random.Next(minColumn, maxColumn + 1);
                var row = _random.Next(minRow, maxRow + 1);

                var newObstacle = new BattleObstacleFields
                {
                    BattleId = battleObject.Id,
                    Width = width,
                    Height = height,
                    Column = column,
                    Row = row,
                    Mask = null,
                };
                fields.Add(newObstacle);
                if (!MapUtils.IsPathAvailable(
                        battleObject,
                        fields,
                        new HexoPoint(0,0),
                        new HexoPoint(battleObject.Width - 1, battleObject.Height - 1),
                        MovementType.Walk,
                        true))
                {
                    fields.Remove(newObstacle);
                }
            }
            
            for (int i = 0; i < _random.Next(minMiddleObstaclesCount, maxMiddleObstaclesCount + 1); i++)
            {
                try
                {
                    var maxWidth = Math.Min(closeToArmy ? battleObject.Width - 2 : battleObject.Width - 4, middleObstacleMaxWidth);
                    var maxHeight = Math.Min(battleObject.Height - 1, middleObstacleMaxHeight);
                
                    var width = _random.Next(middleObstacleMinWidth, maxWidth + 1);
                    var height = _random.Next(middleObstacleMinHeight, maxHeight + 1);

                    var maxColumn = closeToArmy ? battleObject.Width - width - 1 : battleObject.Width - width - 2;
                    var maxRow = battleObject.Height - height;
                
                    var column = _random.Next(minColumn, maxColumn + 1);
                    var row = _random.Next(minRow, maxRow + 1);

                    var newObstacle = new BattleObstacleFields
                    {
                        BattleId = battleObject.Id,
                        Width = width,
                        Height = height,
                        Column = column,
                        Row = row,
                        Mask = null,
                    };
                    fields.Add(newObstacle);
                    if (!MapUtils.IsPathAvailable(
                            battleObject,
                            fields,
                            new HexoPoint(0,0),
                            new HexoPoint(battleObject.Width - 1, battleObject.Height - 1),
                            MovementType.Walk,
                            true))
                    {
                        fields.Remove(newObstacle);
                    }
                }
                catch (Exception e)
                {
                    // ignore
                }
            }
            
            for (int i = 0; i < _random.Next(minLargeObstaclesCount, maxLargeObstaclesCount + 1); i++)
            {
                try
                {
                    var maxWidth = Math.Min(closeToArmy ? battleObject.Width - 2 : battleObject.Width - 4, largeObstacleMaxWidth);
                    var maxHeight = Math.Min(battleObject.Height - 1, largeObstacleMaxHeight);
                
                    var width = _random.Next(largeObstacleMinWidth, maxWidth + 1);
                    var height = _random.Next(largeObstacleMinHeight, maxHeight + 1);

                    var maxColumn = closeToArmy ? battleObject.Width - width - 2 : battleObject.Width - width - 3;
                    var maxRow = battleObject.Height - height;
                
                    var column = _random.Next(minColumn, maxColumn + 1);
                    var row = _random.Next(minRow, maxRow + 1);

                    var newObstacle = new BattleObstacleFields
                    {
                        BattleId = battleObject.Id,
                        Width = width,
                        Height = height,
                        Column = column,
                        Row = row,
                        Mask = null,
                    };
                    fields.Add(newObstacle);
                    if (!MapUtils.IsPathAvailable(
                            battleObject,
                            fields,
                            new HexoPoint(0,0),
                            new HexoPoint(battleObject.Width - 1, battleObject.Height - 1),
                            MovementType.Walk,
                            true))
                    {
                        fields.Remove(newObstacle);
                    }
                }
                catch (Exception e)
                {
                    // ignore
                }
            }
            
            var obstacles = await BattleObstaclesService.CreateBatch(fields);
            return obstacles;
        }
    }
}