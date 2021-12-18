using System;

namespace FriendlyRoguelike.Core.Models
{
    public class GameTimeTicks
    {
        public TimeSpan GameTimeTotal { get; }
        
        public TimeSpan ElapsedFromLastTick { get; } 
        
        public DateTime CurrentLocalTime { get; }

        public GameTimeTicks(DateTime currentLocalTime, TimeSpan gameTimeTotal, TimeSpan elapsedFromLastTick)
        {
            CurrentLocalTime = currentLocalTime;
            GameTimeTotal = gameTimeTotal;
            ElapsedFromLastTick = elapsedFromLastTick;
        }
    }
}