using System;
using Epic.Core.Objects;
using Epic.Data.BattleObstacles;
using NetExtensions.Collections;

namespace Epic.Core.Services.BattleObstacles
{
    public interface IBattleObstacleObject : IGameObject<IBattleObstacleEntity>, IBattleObstacleFields
    {
        Guid Id { get; }
    }
    
    public class MutableBattleObstacleObject : IBattleObstacleObject
    {
        public Guid Id { get; }
        
        public Guid BattleId { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool[,] Mask { get; set; }
        
        private MutableBattleObstacleObject(Guid id)
        {
            Id = id;
        }

        public static MutableBattleObstacleObject FromEntity(IBattleObstacleEntity entity)
        {
            return new MutableBattleObstacleObject(entity.Id)
            {
                BattleId = entity.BattleId,
                Column = entity.Column,
                Row = entity.Row,
                Width = entity.Width,
                Height = entity.Height,
                Mask = entity.Mask ?? CreateFilledMask(entity.Width, entity.Height, true),
            };
        }

        private static bool[,] CreateFilledMask(int width, int height, bool value)
        {
            var mask = new bool[width, height];
            mask.Fill((x, y) => value);
            return mask;
        }
        
        public IBattleObstacleEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}