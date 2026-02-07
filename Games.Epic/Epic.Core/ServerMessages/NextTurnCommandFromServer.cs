using System;

namespace Epic.Core.ServerMessages
{
    public class NextTurnCommandFromServer : PlayerCommandFromServer
    {
        public string NextTurnUnitId { get; }
        public int RoundNumber { get; }
        public bool CanAct { get; }
        
        public NextTurnCommandFromServer(int turnNumber, InBattlePlayerNumber player, Guid nextTurnUnitId, int roundNumber, bool canAct = true)
            : base(turnNumber, player)
        {
            NextTurnUnitId = nextTurnUnitId.ToString();
            RoundNumber = roundNumber;
            CanAct = canAct;
        }

        public override string Command => "NEXT_TURN";
    }
}