namespace Epic.Core.ServerMessages
{
    public class PlayerRansomCommandFromServer : PlayerCommandFromServer
    {
        public PlayerRansomCommandFromServer(int turnNumber, InBattlePlayerNumber player)
            : base(turnNumber, player)
        {
        }

        public override string Command => "PLAYER_RANSOM";
    }
}