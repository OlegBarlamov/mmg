using System;
using JetBrains.Annotations;

namespace Epic.Data.BattleObstacles
{
    public interface IBattleObstacleEntity : IBattleObstacleFields
    {
        Guid Id { get; }
    }

    public interface IBattleObstacleFields
    {
        Guid BattleId { get; }
        int Column { get; }
        int Row { get; }
        int Width { get; }
        int Height { get; }
        [CanBeNull] bool[,] Mask { get; }
    }
    
    public class BattleObstacleFields : IBattleObstacleFields
    {
        public Guid BattleId { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [CanBeNull] public bool[,] Mask { get; set; }
    }

    public class BattleObstacleEntityEntity : BattleObstacleFields, IBattleObstacleEntity
    {
        public Guid Id { get; }

        public BattleObstacleEntityEntity(Guid id, IBattleObstacleFields fields)
        {
            Id = id;
            BattleId = fields.BattleId;
            Column = fields.Column;
            Row = fields.Row;
            Width = fields.Width;
            Height = fields.Height;
            Mask = fields.Mask;
        }
    }
}