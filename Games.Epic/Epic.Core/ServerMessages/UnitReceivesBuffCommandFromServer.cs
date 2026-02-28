namespace Epic.Core.ServerMessages
{
    public class UnitReceivesBuffCommandFromServer : UnitCommandFromServer
    {
        public UnitReceivesBuffCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId) 
            : base(turnNumber, player, actorId)
        {
        }

        public override string Command => "RECEIVE_BUFF";
        
        /// <summary>
        /// The ID of the buff entity that was created.
        /// </summary>
        public string BuffId { get; set; }
        
        /// <summary>
        /// The ID of the buff type (for fetching details via HTTP).
        /// </summary>
        public string BuffTypeId { get; set; }
        
        /// <summary>
        /// The name of the buff for display.
        /// </summary>
        public string BuffName { get; set; }
        
        /// <summary>
        /// The thumbnail URL of the buff for immediate display.
        /// </summary>
        public string ThumbnailUrl { get; set; }
        
        /// <summary>
        /// Whether the buff is permanent.
        /// </summary>
        public bool Permanent { get; set; }
        
        /// <summary>
        /// Whether the buff stuns the unit (cannot move).
        /// </summary>
        public bool Stunned { get; set; }
        
        /// <summary>
        /// Whether the buff paralyzes the unit (cannot act).
        /// </summary>
        public bool Paralyzed { get; set; }
        
        /// <summary>
        /// The remaining duration of the buff in rounds (0 for permanent buffs).
        /// </summary>
        public int DurationRemaining { get; set; }

        /// <summary>
        /// Unit's effective speed after this buff is applied (so client can refresh move highlight).
        /// </summary>
        public int Speed { get; set; }
    }
}
