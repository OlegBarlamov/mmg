using System;
using Epic.Core;
using Epic.Core.Services.Battles;
using Epic.Data.Heroes;

namespace Epic.Server.Resources;

public class PlayerInBattleInfoResource
{
    public Guid PlayerId { get; }
    public bool RansomClaimed { get; }
    public bool RunClaimed { get; }
    public bool MagicUsedThisRound { get; }
    public string PlayerNumber { get; }
    public int Index { get; }
    public HeroStatsResource HeroStats { get; }

    public PlayerInBattleInfoResource(IPlayerInBattleInfoObject playerInBattleInfoObject, InBattlePlayerNumber playerNumber, IHeroStats heroStats, int currentRoundNumber)
    {
        PlayerId = playerInBattleInfoObject.PlayerId;
        RansomClaimed = playerInBattleInfoObject.RansomClaimed;
        RunClaimed = playerInBattleInfoObject.RunClaimed;
        MagicUsedThisRound = playerInBattleInfoObject.LastRoundMagicUsed == currentRoundNumber;
        PlayerNumber = playerNumber.ToString();
        Index = (int)playerNumber;
        HeroStats = new HeroStatsResource(heroStats);
    }
}