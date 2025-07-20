using System;

namespace Epic.Core.ServerMessages
{
    public class NextTurnCommandFromServer : PlayerCommandFromServer
    {
        public string NextTurnUnitId { get; }
        public NextTurnCommandFromServer(int turnNumber, InBattlePlayerNumber player, Guid nextTurnUnitId) : base(turnNumber, player)
        {
            NextTurnUnitId = nextTurnUnitId.ToString();
        }

        public override string Command => "NEXT_TURN";
    }
}