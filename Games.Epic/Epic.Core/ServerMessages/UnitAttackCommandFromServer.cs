namespace Epic.Core.ServerMessages
{
    public class UnitAttackCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "UNIT_ATTACK";
        public string TargetId { get; set; }
    }
}