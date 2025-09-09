namespace Epic.Core.ServerMessages
{
    public class PlayerRunCommandFromServer : PlayerCommandFromServer
    {
        public PlayerRunCommandFromServer(int turnNumber, InBattlePlayerNumber player)
            : base(turnNumber, player)
        {
        }

        public override string Command => "PLAYER_RUN";
    }
}