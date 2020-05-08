using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Services
{
    public interface IDisplayService : IDisposable
    {
        event Action DeviceReset;
        
        GraphicsProfile GraphicsProfile { get; set; }
        bool IsFullScreen { get; set; }
        bool HardwareModeSwitch { get; set; }
        bool PreferMultiSampling { get; set; }
        SurfaceFormat PreferredBackBufferFormat { get; set; }
        int PreferredBackBufferHeight { get; set; }
        int PreferredBackBufferWidth { get; set; }
        DepthFormat PreferredDepthStencilFormat { get; set; }
        bool SynchronizeWithVerticalRetrace { get; set; }
        DisplayOrientation SupportedOrientations { get; set; }
        
        void ToggleFullScreen();
        void ApplyChanges();
    }
}