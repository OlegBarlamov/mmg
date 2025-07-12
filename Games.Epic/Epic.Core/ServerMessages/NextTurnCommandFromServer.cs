namespace Epic.Core.ServerMessages
{
    public class NextTurnCommandFromServer : PlayerCommandFromServer
    {
        public override string Command => "NEXT_TURN";
    }
}