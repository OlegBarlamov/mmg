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
    public string PlayerNumber { get; }
    public int Index { get; }
    public HeroStatsResource HeroStats { get; }
    
    public PlayerInBattleInfoResource(IPlayerInBattleInfoObject playerInBattleInfoObject, InBattlePlayerNumber playerNumber, IHeroStats heroStats)
    {
        PlayerId = playerInBattleInfoObject.PlayerId;
        RansomClaimed = playerInBattleInfoObject.RansomClaimed;
        PlayerNumber = playerNumber.ToString();
        Index = (int)playerNumber;
        RunClaimed = playerInBattleInfoObject.RunClaimed;
        HeroStats = new HeroStatsResource(heroStats);
    }
}