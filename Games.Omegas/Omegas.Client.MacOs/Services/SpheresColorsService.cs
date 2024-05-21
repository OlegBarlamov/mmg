using System;
using Microsoft.Xna.Framework;

namespace Omegas.Client.MacOs.Services
{
    public class SpheresColorsService
    {
        public Color GetPlayerColor(PlayerIndex playerIndex)
        {
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    return Color.Red;
                case PlayerIndex.Two:
                    return Color.Blue;
                case PlayerIndex.Three:
                    return Color.Green;
                case PlayerIndex.Four:
                    return Color.Yellow;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
        }

        public Color GetPlayerHeartColor(PlayerIndex playerIndex)
        {
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    return Color.DarkRed;
                case PlayerIndex.Two:
                    return Color.DarkBlue;
                case PlayerIndex.Three:
                    return Color.DarkGreen;
                case PlayerIndex.Four:
                    return Color.Orange;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
        }

        public Color GetNeutralColor()
        {
            return Color.LightGray;
        }
    }
}