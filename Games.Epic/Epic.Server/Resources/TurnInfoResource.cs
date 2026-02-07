using Epic.Core;

namespace Epic.Server.Resources
{
    public class TurnInfoResource
    {
        public int Index { get; }
        public string Player { get; }
        public int RoundNumber { get; }
        public bool CanAct { get; }
        
        public TurnInfoResource(int index, int playerIndex, int roundNumber, bool canAct = true)
        {
            Index = index;
            Player = ((InBattlePlayerNumber)playerIndex).ToString();
            RoundNumber = roundNumber;
            CanAct = canAct;
        }
    }
}