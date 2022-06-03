using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IMouseProvider
    {
        MouseState Current { get; }
        MouseState Previous { get; }
        
        bool LeftButton { get; }
        
        bool RightButton { get; }
        
        bool LeftButtonPressedOnce { get; }
        
        bool RightButtonPressedOnce { get; }
        
        bool LeftButtonReleasedOnce { get; }
        
        bool RightButtonReleasedOnce { get; }
        
        Point PositionDelta { get; }
        
        Point Position { get; }

        void SetPosition(Point point);
    }
}