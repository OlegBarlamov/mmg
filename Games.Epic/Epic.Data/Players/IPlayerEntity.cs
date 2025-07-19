using System;

namespace Epic.Data.Players
{
    public interface IPlayerEntity : IPlayerEntityFields
    {
        Guid Id { get; }
    }

    public interface IPlayerEntityFields
    {
        Guid UserId { get; }
        int Day { get; }
        string Name { get; }
        PlayerEntityType PlayerType { get; }
        bool IsDefeated { get; }
    }

    public class MutablePlayerEntityFields : IPlayerEntityFields
    {
        public Guid UserId { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public PlayerEntityType PlayerType { get; set; }
        public bool IsDefeated { get; set; }
    }
    
    public class MutablePlayerEntity : MutablePlayerEntityFields, IPlayerEntity
    {
        public Guid Id { get; set; }
    }
}