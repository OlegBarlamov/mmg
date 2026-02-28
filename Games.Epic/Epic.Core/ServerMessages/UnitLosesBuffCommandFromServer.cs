namespace Epic.Core.ServerMessages
{
    public class UnitLosesBuffCommandFromServer : UnitCommandFromServer
    {
        public UnitLosesBuffCommandFromServer(int turnNumber, InBattlePlayerNumber player, string actorId) 
            : base(turnNumber, player, actorId)
        {
        }

        public override string Command => "LOSE_BUFF";
        
        /// <summary>
        /// The ID of the buff entity that was removed.
        /// </summary>
        public string BuffId { get; set; }
        
        /// <summary>
        /// The name of the buff for display purposes.
        /// </summary>
        public string BuffName { get; set; }

        /// <summary>
        /// Unit's effective speed after this buff is removed (so client can refresh move highlight).
        /// </summary>
        public int Speed { get; set; }
    }
}
