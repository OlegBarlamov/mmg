using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Magic;
using Epic.Core;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.MagicTypes;
using Epic.Data.MagicType;
using Epic.Logic.Battle.Map;

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

            if (magicType.CastTargetType == CastTargetType.Enemy || magicType.CastTargetType == CastTargetType.Ally)
            {
                if (!command.TargetUnitId.HasValue || command.TargetUnitId.Value == Guid.Empty)
                    throw new BattleLogicException($"Magic {magicType.Name} requires a target unit");
                var targetUnit = context.BattleObject.Units.FirstOrDefault(u => u.Id == command.TargetUnitId.Value);
                if (targetUnit == null)
                    throw new BattleLogicException("Target unit not found");
                if (!targetUnit.GlobalUnit.IsAlive)
                    throw new BattleLogicException("Target unit is not alive");
                var casterPlayerIndex = (int)command.Player;
                if (magicType.CastTargetType == CastTargetType.Enemy && targetUnit.PlayerIndex == casterPlayerIndex)
                    throw new BattleLogicException("Target must be an enemy");
                if (magicType.CastTargetType == CastTargetType.Ally && targetUnit.PlayerIndex != casterPlayerIndex)
                    throw new BattleLogicException("Target must be an ally");
            }

            if (magicType.CastTargetType == CastTargetType.Location)
            {
                if (!command.TargetRow.HasValue || !command.TargetColumn.HasValue)
                    throw new BattleLogicException($"Magic {magicType.Name} requires a target cell (row and column)");
                if (command.TargetRow.Value < 0 || command.TargetRow.Value >= context.BattleObject.Height
                    || command.TargetColumn.Value < 0 || command.TargetColumn.Value >= context.BattleObject.Width)
                    throw new BattleLogicException("Target cell is out of map bounds");
            }
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, PlayerMagicClientBattleMessage command)
        {
            var magicType = await context.MagicTypesService.GetById(command.MagicTypeId);
            var manaCost = magicType?.MannaCost ?? 0;

            var player = await context.PlayersService.GetById(_playerId);
            var hero = player?.ActiveHero;
            if (hero != null && manaCost > 0)
                await context.HeroesService.SpendMana(hero.Id, manaCost);

            var variables = MagicExpressionsVariables.FromHero(hero?.GetCumulativeHeroStats());
            var magic = await context.MagicsService.Create(command.MagicTypeId, variables);

            var targets = ResolveTargets(context, command, magic.MagicType);
            var turnIndex = command.TurnIndex;
            var playerNumber = command.Player;

            foreach (var target in targets)
            {
                if (!target.GlobalUnit.IsAlive)
                    continue;

                if (magic.ApplyBuffs != null && magic.ApplyBuffs.Count > 0)
                    await context.BuffsLogic.ApplyMagicBuffsToTarget(target, magic.ApplyBuffs, turnIndex, playerNumber);

                if (magic.ApplyEffects != null && magic.ApplyEffects.Count > 0)
                {
                    await context.BuffsLogic.ApplyEffectPropertiesToUnit(
                        target,
                        magic.ApplyEffects,
                        turnIndex,
                        playerNumber);
                }
            }

            var playerInfo = context.BattleObject.PlayerInfos.First(x => x.PlayerId == _playerId);
            var mutableInfo = (MutablePlayerInBattleInfoObject)playerInfo;
            mutableInfo.LastRoundMagicUsed = context.BattleObject.RoundNumber;

            await context.BattlesService.UpdateInBattlePlayerInfo(mutableInfo);

            var serverCommand = new PlayerMagicCommandFromServer(command.TurnIndex, command.Player, command.MagicTypeId);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);

            return new CmdExecutionResult(false);
        }

        private static List<MutableBattleUnitObject> ResolveTargets(
            CommandExecutionContext context,
            PlayerMagicClientBattleMessage command,
            IMagicTypeObject magicType)
        {
            var castTargetType = magicType.CastTargetType;
            var effectRadius = magicType.EffectRadius;
            var units = context.BattleObject.Units;
            var casterPlayerIndex = (int)command.Player;
            var alive = units.Where(u => u.GlobalUnit.IsAlive).Cast<MutableBattleUnitObject>().ToList();

            switch (castTargetType)
            {
                case CastTargetType.Enemy:
                    if (!command.TargetUnitId.HasValue || command.TargetUnitId.Value == Guid.Empty)
                        return new List<MutableBattleUnitObject>();
                    return GetUnitsInRadiusWithSideFilter(
                        context, alive, command.TargetUnitId.Value,
                        effectRadius, casterPlayerIndex, enemyOnly: true);

                case CastTargetType.Ally:
                    if (!command.TargetUnitId.HasValue || command.TargetUnitId.Value == Guid.Empty)
                        return new List<MutableBattleUnitObject>();
                    return GetUnitsInRadiusWithSideFilter(
                        context, alive, command.TargetUnitId.Value,
                        effectRadius, casterPlayerIndex, enemyOnly: false);

                case CastTargetType.AllEnemies:
                    return alive.Where(u => u.PlayerIndex != casterPlayerIndex).ToList();

                case CastTargetType.AllAllies:
                    return alive.Where(u => u.PlayerIndex == casterPlayerIndex).ToList();

                case CastTargetType.AllUnits:
                    return alive;

                case CastTargetType.Location:
                    if (!command.TargetRow.HasValue || !command.TargetColumn.HasValue)
                        return new List<MutableBattleUnitObject>();
                    var centerCell = new HexoPoint(command.TargetColumn.Value, command.TargetRow.Value);
                    return GetUnitsInRadiusAtCell(context, alive, centerCell, effectRadius);

                default:
                    return new List<MutableBattleUnitObject>();
            }
        }

        /// <summary>Enemy: all enemy units within radius of the chosen unit. Ally: all ally units within radius of the chosen unit.</summary>
        private static List<MutableBattleUnitObject> GetUnitsInRadiusWithSideFilter(
            CommandExecutionContext context,
            List<MutableBattleUnitObject> alive,
            Guid centerUnitId,
            int effectRadius,
            int casterPlayerIndex,
            bool enemyOnly)
        {
            var centerUnit = alive.FirstOrDefault(u => u.Id == centerUnitId);
            if (centerUnit == null)
                return new List<MutableBattleUnitObject>();
            var cellsInRadius = GetCellsInRadius(context, new HexoPoint(centerUnit.Column, centerUnit.Row), effectRadius);
            return alive.Where(u =>
                (enemyOnly ? u.PlayerIndex != casterPlayerIndex : u.PlayerIndex == casterPlayerIndex)
                && cellsInRadius.Any(c => c.R == u.Row && c.C == u.Column)).ToList();
        }

        /// <summary>Location: all units (any side) within radius of the target cell.</summary>
        private static List<MutableBattleUnitObject> GetUnitsInRadiusAtCell(
            CommandExecutionContext context,
            List<MutableBattleUnitObject> alive,
            HexoPoint centerCell,
            int effectRadius)
        {
            var cellsInRadius = GetCellsInRadius(context, centerCell, effectRadius);
            return alive.Where(u => cellsInRadius.Any(c => c.R == u.Row && c.C == u.Column)).ToList();
        }

        private static List<HexoPoint> GetCellsInRadius(
            CommandExecutionContext context,
            HexoPoint center,
            int effectRadius)
        {
            if (effectRadius <= 0)
                return new List<HexoPoint> { center };
            return MapUtils.GetCellsWithinRadius(
                center,
                effectRadius,
                context.BattleObject.Height,
                context.BattleObject.Width,
                includeCenter: true);
        }
    }
}
