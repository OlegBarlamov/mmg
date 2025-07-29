namespace Epic.Core.ServerMessages
{
    public class UnitPassCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "UNIT_PASS";
        
        public UnitPassCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId)
            : base(turnNumber, player, actorId)
        {
        }
    }
}
