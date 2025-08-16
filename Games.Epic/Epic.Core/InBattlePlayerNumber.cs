namespace Epic.Core
{
    public enum InBattlePlayerNumber
    {
        Unknown = 0,
        Player1 = 1,
        Player2 = 2
    }

    public static class InBattlePlayerNumberExtensions
    {
        public static InBattlePlayerNumber ToInBattlePlayerNumber(this int value)
        {
            return (InBattlePlayerNumber)value;
        }
    }
}