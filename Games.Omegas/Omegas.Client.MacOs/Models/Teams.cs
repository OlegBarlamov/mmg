using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Omegas.Client.MacOs.Models
{
    public enum Teams
    {
        Neutral,
        Player1,
        Player2,
        Player1Ally,
        Player2Ally,
    }

    public enum TeamsRelation
    {
        Self,
        Neutral,
        Ally,
        Enemy
    }

    public static class TeamsExtensions
    {
        public static Teams ToTeam(this PlayerIndex playerIndex)
        {
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    return Teams.Player1;
                case PlayerIndex.Two:
                    return Teams.Player2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
        }
        
        public static bool IsAllyWith(this Teams teamA, Teams teamB)
        {
            return teamA.Relation(teamB) == TeamsRelation.Ally;
        }

        public static bool IsSelf(this Teams teamA, Teams teamB)
        {
            return teamA.Relation(teamB) == TeamsRelation.Self;
        }
        
        public static bool IsEnemyWith(this Teams teamA, Teams teamB)
        {
            return teamA.Relation(teamB) == TeamsRelation.Enemy;
        }

        public static bool IsNeutralWith(this Teams teamA, Teams teamB)
        {
            return teamA.Relation(teamB) == TeamsRelation.Neutral;
        }
        
        public static TeamsRelation Relation(this Teams teamA, Teams teamB)
        {
            var code = GetKey(teamA, teamB);
            return Relations[code];
        }
        
        private static int GetKey(Teams typeA, Teams typeB)
        {
            unchecked
            {
                int hash1 = (int)typeA;
                int hash2 = (int)typeB;
                return (hash1 * 397) ^ hash2;
            }
        }
        
        private static readonly Dictionary<int, TeamsRelation> Relations = new Dictionary<int, TeamsRelation>
        {
            { GetKey(Teams.Neutral, Teams.Player1), TeamsRelation.Neutral },
            { GetKey(Teams.Neutral, Teams.Player2), TeamsRelation.Neutral },
            { GetKey(Teams.Neutral, Teams.Neutral), TeamsRelation.Self },
            { GetKey(Teams.Neutral, Teams.Player1Ally), TeamsRelation.Neutral },
            { GetKey(Teams.Neutral, Teams.Player2Ally), TeamsRelation.Neutral },
            
            { GetKey(Teams.Player1, Teams.Player1), TeamsRelation.Self },
            { GetKey(Teams.Player1, Teams.Player2), TeamsRelation.Enemy },
            { GetKey(Teams.Player1, Teams.Neutral), TeamsRelation.Neutral },
            { GetKey(Teams.Player1, Teams.Player1Ally), TeamsRelation.Ally },
            { GetKey(Teams.Player1, Teams.Player2Ally), TeamsRelation.Enemy },
            
            { GetKey(Teams.Player2, Teams.Player1), TeamsRelation.Enemy },
            { GetKey(Teams.Player2, Teams.Player2), TeamsRelation.Self },
            { GetKey(Teams.Player2, Teams.Neutral), TeamsRelation.Neutral },
            { GetKey(Teams.Player2, Teams.Player1Ally), TeamsRelation.Enemy },
            { GetKey(Teams.Player2, Teams.Player2Ally), TeamsRelation.Ally },
            
            { GetKey(Teams.Player1Ally, Teams.Player1), TeamsRelation.Ally },
            { GetKey(Teams.Player1Ally, Teams.Player2), TeamsRelation.Enemy },
            { GetKey(Teams.Player1Ally, Teams.Neutral), TeamsRelation.Neutral },
            { GetKey(Teams.Player1Ally, Teams.Player1Ally), TeamsRelation.Self },
            { GetKey(Teams.Player1Ally, Teams.Player2Ally), TeamsRelation.Enemy },
            
            { GetKey(Teams.Player2Ally, Teams.Player1), TeamsRelation.Enemy },
            { GetKey(Teams.Player2Ally, Teams.Player2), TeamsRelation.Ally },
            { GetKey(Teams.Player2Ally, Teams.Neutral), TeamsRelation.Neutral },
            { GetKey(Teams.Player2Ally, Teams.Player1Ally), TeamsRelation.Enemy },
            { GetKey(Teams.Player2Ally, Teams.Player2Ally), TeamsRelation.Self },
        }; 
        
    }
}