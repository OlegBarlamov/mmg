namespace Epic.Core.ServerMessages
{
    public class UnitHealsCommandFromServer : UnitCommandFromServer
    {
        public UnitHealsCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId) 
            : base(turnNumber, player, actorId)
        {
        }

        public override string Command => "UNIT_HEALS";
        
        /// <summary>
        /// The amount of HP healed.
        /// </summary>
        public int HealedAmount { get; set; }
        
        /// <summary>
        /// The number of units resurrected (if any).
        /// </summary>
        public int ResurrectedCount { get; set; }
        
        /// <summary>
        /// The new total count of units in the stack.
        /// </summary>
        public int NewCount { get; set; }
        
        /// <summary>
        /// The new health of the top unit in the stack.
        /// </summary>
        public int NewHealth { get; set; }
    }
}
