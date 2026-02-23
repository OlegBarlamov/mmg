using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.MagicTypes;

namespace Epic.Logic.Battle.Commands
{
    internal class PlayerMagicHandler : BaseTypedCommandHandler<PlayerMagicClientBattleMessage>
    {
        private Guid _playerId;

        public override async Task Validate(CommandExecutionContext context, PlayerMagicClientBattleMessage command)
        {
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);

            var playerId = context.BattleObject.FindPlayerId(command.Player);
            if (!playerId.HasValue)
                throw new BattleLogicException($"Unknown player {command.Player}");

            _playerId = playerId.Value;
            var playerInfo = context.BattleObject.PlayerInfos.First(x => x.PlayerId == _playerId);

            if (playerInfo.LastRoundMagicUsed == context.BattleObject.RoundNumber)
                throw new BattleLogicException($"Player {command.Player} has already used magic this round");

            if (playerInfo.RansomClaimed)
                throw new BattleLogicException($"Player {command.Player} has already claimed ransom");
            if (playerInfo.RunClaimed)
                throw new BattleLogicException($"Player {command.Player} has already claimed run");

            var magicType = await context.MagicTypesService.GetById(command.MagicTypeId);
            if (magicType == null)
                throw new BattleLogicException($"Unknown magic type {command.MagicTypeId}");

            var player = await context.PlayersService.GetById(_playerId);
            var hero = player?.ActiveHero;
            if (hero == null)
                throw new BattleLogicException($"Player {command.Player} has no active hero");

            if (!hero.KnownMagicTypeIds?.Contains(command.MagicTypeId) ?? true)
                throw new BattleLogicException($"Hero does not know magic type {magicType.Name ?? command.MagicTypeId.ToString()}");

            var manaCost = magicType.MannaCost;
            if (hero.CurrentMana < manaCost)
                throw new BattleLogicException($"Not enough mana (have {hero.CurrentMana}, need {manaCost})");
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, PlayerMagicClientBattleMessage command)
        {
            var magicType = await context.MagicTypesService.GetById(command.MagicTypeId);
            var manaCost = magicType?.MannaCost ?? 0;

            var player = await context.PlayersService.GetById(_playerId);
            if (player?.ActiveHero != null && manaCost > 0)
                await context.HeroesService.SpendMana(player.ActiveHero.Id, manaCost);

            var playerInfo = context.BattleObject.PlayerInfos.First(x => x.PlayerId == _playerId);
            var mutableInfo = (MutablePlayerInBattleInfoObject)playerInfo;
            mutableInfo.LastRoundMagicUsed = context.BattleObject.RoundNumber;

            await context.BattlesService.UpdateInBattlePlayerInfo(mutableInfo);

            var serverCommand = new PlayerMagicCommandFromServer(command.TurnIndex, command.Player, command.MagicTypeId);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);

            return new CmdExecutionResult(false);
        }
    }
}
