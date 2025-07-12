namespace Epic.Core.ServerMessages
{
    public class UnitTakeDamageCommandFromServer : UnitCommandFromServer
    {
        public override string Command => "TAKE_DAMAGE";
        public int DamageTaken { get; set; }
        public int KilledCount { get; set; }
        public int RemainingCount { get; set; }
        public int RemainingHealth { get; set; }
    }
}