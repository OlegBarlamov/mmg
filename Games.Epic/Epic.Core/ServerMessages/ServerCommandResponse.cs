using Epic.Core.ClientMessages;

namespace Epic.Core.ServerMessages
{
    public abstract class ServerCommandResponse : BaseCommandFromServer
    {
        public IClientBattleMessage RequestedCommand { get; }
        public abstract string Status { get; }
        public string RejectedReason { get; set; }
        public string RejectedReasonDetails { get; set; }
        public override string Command { get; } = "RESPONSE";

        protected ServerCommandResponse(int turnNumber, IClientBattleMessage requestedCommand) : base(turnNumber)
        {
            RequestedCommand = requestedCommand;
        }
    }

    public class CommandApproved : ServerCommandResponse
    {
        public override string Status => "Approved";
        
        public CommandApproved(IClientBattleMessage requestedCommand)
            : base(requestedCommand.TurnIndex, requestedCommand)
        {
        }
    }
    
    public class CommandRejected : ServerCommandResponse
    {
        public override string Status => "Rejected";

        public CommandRejected(IClientBattleMessage requestedCommand, string rejectedReason, string rejectedReasonDetails)
            : base(requestedCommand.TurnIndex, requestedCommand)
        {
            RejectedReason = rejectedReason;
            RejectedReasonDetails = rejectedReasonDetails;
        }
    }
}