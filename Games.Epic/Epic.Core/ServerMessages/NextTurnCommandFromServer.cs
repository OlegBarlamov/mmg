using System;

namespace Epic.Core.ServerMessages
{
    public class NextTurnCommandFromServer : PlayerCommandFromServer
    {
        public string NextTurnUnitId { get; }
        public int RoundNumber { get; }
        public NextTurnCommandFromServer(int turnNumber, InBattlePlayerNumber player, Guid nextTurnUnitId, int roundNumber)
            : base(turnNumber, player)
        {
            NextTurnUnitId = nextTurnUnitId.ToString();
            RoundNumber = roundNumber;
        }

        public override string Command => "NEXT_TURN";
    }
}