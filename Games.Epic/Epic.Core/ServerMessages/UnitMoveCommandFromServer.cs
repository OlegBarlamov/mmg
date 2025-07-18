namespace Epic.Core.ServerMessages
{
    public class UnitMoveCommandFromServer : UnitCommandFromServer
    {
        public UnitMoveCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId, HexoPoint moveToCell) : base(turnNumber, player, actorId)
        {
            MoveToCell = moveToCell;
        }

        public override string Command => "UNIT_MOVE";
        public HexoPoint MoveToCell { get; }
    }
}