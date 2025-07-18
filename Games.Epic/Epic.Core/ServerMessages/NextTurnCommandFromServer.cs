namespace Epic.Core.ServerMessages
{
    public class NextTurnCommandFromServer : PlayerCommandFromServer
    {
        public NextTurnCommandFromServer(int turnNumber, InBattlePlayerNumber player) : base(turnNumber, player)
        {
        }

        public override string Command => "NEXT_TURN";
    }
}