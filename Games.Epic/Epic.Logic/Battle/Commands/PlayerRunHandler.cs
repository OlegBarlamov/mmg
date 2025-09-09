using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;

namespace Epic.Logic.Battle.Commands
{
    internal class PlayerRunHandler : BaseTypedCommandHandler<PlayerRunClientBattleMessage>
    {
        private Guid _playerId;
        
        public override Task Validate(CommandExecutionContext context, PlayerRunClientBattleMessage command)
        {
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
            
            var playerId = context.BattleObject.FindPlayerId(command.Player);
            if (!playerId.HasValue)
                throw new BattleLogicException($"Unknown player {command.Player}");

            _playerId = playerId.Value;
            var playerInfo = context.BattleObject.PlayerInfos.First(x => x.PlayerId == _playerId);
            if (playerInfo.RunClaimed)
                throw new BattleLogicException($"Player {command.Player} has already claimed run");
            if (playerInfo.RansomClaimed)
                throw new BattleLogicException($"Player {command.Player} has already claimed ransom");
            
            return Task.CompletedTask;
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, PlayerRunClientBattleMessage command)
        {
            var playerInfo = context.BattleObject.PlayerInfos.First(x => x.PlayerId == _playerId);
            playerInfo.RunClaimed = true;

            var playerIndex = (int)context.BattleObject.FindPlayerNumber(playerInfo);
            var playerMinSpeed = context.BattleObject.Units
                .Where(x => x.GlobalUnit.IsAlive && x.PlayerIndex == playerIndex)
                .Min(x => x.GlobalUnit.UnitType.Speed);
            
            var enemyMaxSpeed = context.BattleObject.Units
                .Where(x => x.GlobalUnit.IsAlive && x.PlayerIndex != playerIndex)
                .Max(x => x.GlobalUnit.UnitType.Speed);

            playerInfo.RunInfo = new PlayerRunInfo
            {
                RunClaimedUnitIndex = 0,
                RunRoundsRemaining = enemyMaxSpeed - playerMinSpeed + 1,
            };
            
            await context.BattlesService.UpdateInBattlePlayerInfo(playerInfo);
            
            var serverCommand = new PlayerRunCommandFromServer(command.TurnIndex, command.Player);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);

            return new CmdExecutionResult(true);
        }
    }
}