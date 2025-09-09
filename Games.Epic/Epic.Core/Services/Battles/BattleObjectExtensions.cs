using System;
using System.Linq;
using JetBrains.Annotations;

namespace Epic.Core.Services.Battles
{
    public static class BattleObjectExtensions
    {
        public static Guid? FindPlayerId(this IBattleObject battleObject, InBattlePlayerNumber playerNumber)
        {
            int index = (int)playerNumber - 1;
            if (index < 0 || index >= battleObject.PlayerInfos.Count)
                return null;
            
            return battleObject.PlayerInfos[index].PlayerId;
        }

        public static InBattlePlayerNumber? FindPlayerNumber(this IBattleObject battleObject,
            IPlayerInBattleInfoObject playerInfo)
        {
            var index = Array.IndexOf(battleObject.PlayerInfos.ToArray(), playerInfo);
            if (index < 0)
                return null;

            return (InBattlePlayerNumber)(index + 1);
        }

        [CanBeNull]
        public static IPlayerInBattleInfoObject FindPlayerInfo(this IBattleObject battleObject, InBattlePlayerNumber playerNumber)
        {
            int index = (int)playerNumber - 1;
            if (index < 0 || index >= battleObject.PlayerInfos.Count)
                return null;
            
            return battleObject.PlayerInfos[index];
        }
    }
}