using Epic.Core;

namespace Epic.Server.Resources
{
    public class TurnInfoResource
    {
        public int Index { get; }
        public string Player { get; }
        public int RoundNumber { get; }
        
        public TurnInfoResource(int index, int playerIndex, int roundNumber)
        {
            Index = index;
            Player = ((InBattlePlayerNumber)playerIndex).ToString();
            RoundNumber = roundNumber;
        }
    }
}