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

            var minObstaclesCount = 0;
            var maxObstaclesCount = 0;
            switch (stage)
            {
                case 1:
                    minObstaclesCount = 0;
                    maxObstaclesCount = 0;
                    break;
                case 2:
                    minObstaclesCount = 1;
                    maxObstaclesCount = 2;
                    break;
                case 3:
                    minObstaclesCount = 3;
                    maxObstaclesCount = 5;
                    break;
                case 4:
                    minObstaclesCount = 6;
                    maxObstaclesCount = 10;
                    break;;
            }

            var closeToArmy = _random.Next(100) < 20;
            var minColumn = closeToArmy ? 1 : 2;
            var minRow = 0;
            var maxWidth = Math.Min(closeToArmy ? battleObject.Width - 2 : battleObject.Width - 4, 7);
            var maxHeight = Math.Min(battleObject.Height - 1, 4);

            var fields = new List<IBattleObstacleFields>();
            var obstaclesCount = _random.Next(minObstaclesCount, maxObstaclesCount + 1);
            for (int i = 0; i < obstaclesCount; i++)
            {
                var width = _random.Next(1, maxWidth + 1);
                var height = _random.Next(1, maxHeight + 1);
                
                var maxColumn = maxWidth - width + 1;
                var maxRow = maxHeight  - height + 1;
                
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
            
            var obstacles = await BattleObstaclesService.CreateBatch(fields);
            return obstacles;
        }
    }
}