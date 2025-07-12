namespace Epic.Core.ServerMessages
{
    public class UnitMoveCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "UNIT_MOVE";
        public HexoPoint MoveToCell { get; set; }
    }
}