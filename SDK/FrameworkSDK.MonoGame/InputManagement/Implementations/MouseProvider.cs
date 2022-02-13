using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class MouseProvider : IMouseProvider, IUpdatable
    {
        public MouseState Current { get; private set; } = Mouse.GetState();
        public MouseState Previous { get; private set; } = Mouse.GetState();
        
        public bool LeftButton { get; private set;}
        public bool RightButton { get;private set; }
        public bool LeftButtonPressedOnce { get;private set; }
        public bool RightButtonPressedOnce { get;private set; }
        public bool LeftButtonReleasedOnce { get;private set; }
        public bool RightButtonReleasedOnce { get;private set; }

        public MouseProvider()
        {
            OneMouseStateUpdated();
        }
        
        public void Update(GameTime gameTime)
        {
            Previous = Current;
            Current = Mouse.GetState();
            
            OneMouseStateUpdated();
        }

        private void OneMouseStateUpdated()
        {
            LeftButton = IsPressed(Current.LeftButton);
            RightButton = IsPressed(Current.RightButton);

            LeftButtonPressedOnce = IsPressed(Current.LeftButton) && IsReleased(Previous.LeftButton);
            RightButtonPressedOnce = IsPressed(Current.RightButton) && IsReleased(Previous.RightButton);
            
            LeftButtonReleasedOnce = IsReleased(Current.LeftButton) && IsPressed(Previous.LeftButton);
            RightButtonReleasedOnce = IsReleased(Current.RightButton) && IsPressed(Previous.RightButton);
        }

        private static bool IsPressed(ButtonState buttonState)
        {
            return buttonState == ButtonState.Pressed;
        }
        
        private static bool IsReleased(ButtonState buttonState)
        {
            return buttonState == ButtonState.Released;
        }
    }
}