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
        /// The name of the buff for display purposes.
        /// </summary>
        public string BuffName { get; set; }
        
        /// <summary>
        /// Whether this buff is permanent (never expires).
        /// </summary>
        public bool Permanent { get; set; }
        
        /// <summary>
        /// The remaining duration of the buff in rounds (0 for permanent buffs).
        /// </summary>
        public int DurationRemaining { get; set; }
    }
}
