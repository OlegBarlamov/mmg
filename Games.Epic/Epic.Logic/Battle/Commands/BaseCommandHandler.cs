using System.Linq;
using Epic.Core;
using Epic.Core.Logic.Erros;
using Epic.Core.Services.Battles;

namespace Epic.Logic.Battle.Commands
{
    
    internal abstract class BaseCommandHandler
    {
        protected class CmdExecutionResult : ICmdExecutionResult
        {
            public bool TurnFinished { get; set; }

            public CmdExecutionResult(bool turnFinished)
            {
                TurnFinished = turnFinished;
            }
        }
        
        protected MutableBattleUnitObject TargetActor { get; private set; }
        
        protected void ValidateTargetActor(CommandExecutionContext context, string actorId)
        {
            TargetActor =
                context.BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == actorId);
            if (TargetActor == null)
                throw new BattleLogicException("Not found target actor for client command");
            if (TargetActor.Id != context.UnitsCarousel.ActiveUnit.Id)
                throw new BattleLogicException("Wrong target actor for client command");
        }

        protected void ValidateExpectedTurn(CommandExecutionContext context, int turnIndex, InBattlePlayerNumber player)
        {
            if (turnIndex != context.ExpectedTurn.TurnIndex || (int)player != context.ExpectedTurn.PlayerIndex) 
                throw new BattleLogicException("Wrong turn index or player index");
        }
    }
}