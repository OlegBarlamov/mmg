using Epic.Data.Players;

namespace Epic.Core.Services.Players
{
    public enum PlayerObjectType {
        Human,
        Computer,
    }

    public static class PlayerObjectTypeExtensions
    {
        public static PlayerEntityType ToEntity(this PlayerObjectType playerObjectType)
        {
            return (PlayerEntityType)playerObjectType;
        }
        
        public static PlayerObjectType ToObjectType(this PlayerEntityType entityType)
        {
            return (PlayerObjectType)entityType;
        }
    }
}