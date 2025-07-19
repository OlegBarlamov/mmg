using System;
using Epic.Data.Players;

namespace Epic.Core.Services.Players
{
    public interface IPlayerObject
    {
        Guid Id { get; }
        Guid UserId { get; }
        int Day { get; }
        string Name { get; }
        PlayerObjectType PlayerType { get; }
        bool IsDefeated { get; }
    }

    internal class MutablePlayerObject : IPlayerObject
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public PlayerObjectType PlayerType { get; set; }
        public bool IsDefeated { get; set; }
        
        private MutablePlayerObject() {}

        public static MutablePlayerObject FromEntity(IPlayerEntity entity)
        {
            return new MutablePlayerObject
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Day = entity.Day,
                Name = entity.Name,
                PlayerType = entity.PlayerType.ToObjectType(),
                IsDefeated = entity.IsDefeated,
            };
        }

        public static IPlayerEntity ToEntity(MutablePlayerObject mutablePlayerObject)
        {
            return new MutablePlayerEntity
            {
                Id = mutablePlayerObject.Id,
                UserId = mutablePlayerObject.UserId,
                Day = mutablePlayerObject.Day,
                Name = mutablePlayerObject.Name,
                PlayerType = mutablePlayerObject.PlayerType.ToEntity(),
                IsDefeated = mutablePlayerObject.IsDefeated,
            };
        }
    }
}
