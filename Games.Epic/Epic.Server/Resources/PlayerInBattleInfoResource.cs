using System;
using Epic.Core;
using Epic.Core.Services.Battles;

namespace Epic.Server.Resources;

public class PlayerInBattleInfoResource
{
    public Guid PlayerId { get; }
    public bool RansomClaimed { get; }
    public string PlayerNumber { get; }
    public int Index { get; }
    
    public PlayerInBattleInfoResource(IPlayerInBattleInfoObject playerInBattleInfoObject, InBattlePlayerNumber playerNumber)
    {
        PlayerId = playerInBattleInfoObject.PlayerId;
        RansomClaimed = playerInBattleInfoObject.RansomClaimed;
        PlayerNumber = playerNumber.ToString();
        Index = (int)playerNumber;
    }
}