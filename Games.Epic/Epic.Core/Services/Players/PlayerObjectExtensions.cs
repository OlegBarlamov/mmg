namespace Epic.Core.Services.Players
{
    public static class PlayerObjectExtensions
    {
        public static bool HasActiveAliveHero(this IPlayerObject playerObject)
        {
            return playerObject.ActiveHero != null && !playerObject.ActiveHero.IsKilled;
        }
    }
}