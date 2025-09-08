using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.GameResources.Errors;
using Epic.Data.GameResources;

namespace Epic.Logic.Battle.Commands
{
    internal class PlayerRansomHandler : BaseTypedCommandHandler<PlayerRansomClientBattleMessage>
    {
        private Guid _playerId;
        private Price _priceToPay;
        public override async Task Validate(CommandExecutionContext context, PlayerRansomClientBattleMessage command)
        {
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
            
            var playerId = context.BattleObject.GetPlayerId(command.Player);
            if (!playerId.HasValue)
                throw new BattleLogicException($"Unknown player {command.Player}");

            _playerId = playerId.Value;
            var ransomToPay = await context.BattlesService.CalculateRansomValueForPlayer(_playerId, context.BattleObject);
            _priceToPay = Price.Create(new Dictionary<Guid, int>
            {
                { context.GameResourcesRepository.GoldResourceId, ransomToPay },
            });
            var isEnoughToPay = await context.GameResourcesRepository.IsEnoughToPay(_priceToPay, _playerId);
            if (!isEnoughToPay)
                throw new NotEnoughResourcesToPayException();
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, PlayerRansomClientBattleMessage command)
        {
            await context.GameResourcesRepository.Pay(_priceToPay, _playerId);

            var serverCommand = new PlayerRansomCommandFromServer(command.TurnIndex, command.Player);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);

            return new CmdExecutionResult(true);
        }
    }
}