using Microsoft.Xna.Framework;
using RectangleF = MonoGameExtensions.Geometry.RectangleF;

namespace FrameworkSDK.MonoGame.Services.Extensions
{
    public static class DisplayServiceExtensions
    {
        public static Rectangle GetDisplayRectangle(this IDisplayService displayService)
        {
            return new Rectangle(0, 0, displayService.PreferredBackBufferWidth, displayService.PreferredBackBufferHeight);
        }
        
        public static RectangleF GetDisplayFRectangle(this IDisplayService displayService)
        {
            return new RectangleF(0, 0, displayService.PreferredBackBufferWidth, displayService.PreferredBackBufferHeight);
        }
        
    }
}