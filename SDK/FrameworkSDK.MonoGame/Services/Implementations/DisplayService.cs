using System;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class DisplayService : IDisplayService
    {
        public event Action DeviceReset;

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return null;

                return GameHeartServices.GraphicsDeviceManager.GraphicsDevice;
            }
        }

        public GraphicsProfile GraphicsProfile
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.GraphicsProfile;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.GraphicsProfile = value;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return false;

                return GameHeartServices.GraphicsDeviceManager.IsFullScreen;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.IsFullScreen = value;
            }
        }

        public bool HardwareModeSwitch
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return false;

                return GameHeartServices.GraphicsDeviceManager.HardwareModeSwitch;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.HardwareModeSwitch = value;
            }
        }

        public bool PreferMultiSampling
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return false;

                return GameHeartServices.GraphicsDeviceManager.PreferMultiSampling;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.PreferMultiSampling = value;
            }
        }

        public SurfaceFormat PreferredBackBufferFormat
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.PreferredBackBufferFormat;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.PreferredBackBufferFormat = value;
            }
        }

        public int PreferredBackBufferHeight
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.PreferredBackBufferHeight;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.PreferredBackBufferHeight = value;
            }
        }

        public int PreferredBackBufferWidth
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.PreferredBackBufferWidth;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.PreferredBackBufferWidth = value;
            }
        }

        public DepthFormat PreferredDepthStencilFormat
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.PreferredDepthStencilFormat;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.PreferredDepthStencilFormat = value;
            }
        }

        public bool SynchronizeWithVerticalRetrace
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return false;

                return GameHeartServices.GraphicsDeviceManager.SynchronizeWithVerticalRetrace;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
            }
        }

        public DisplayOrientation SupportedOrientations
        {
            get
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return 0;

                return GameHeartServices.GraphicsDeviceManager.SupportedOrientations;
            }
            set
            {
                if (!GameHeartServices.IsServicesLoaded)
                    return;

                GameHeartServices.GraphicsDeviceManager.SupportedOrientations = value;
            }
        }

        private IGameHeartServices GameHeartServices { get; }


        public DisplayService([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            
            GameHeartServices.Loaded += GameHeartServicesOnLoaded;
        }
        
        public void Dispose()
        {
            GameHeartServices.Loaded -= GameHeartServicesOnLoaded;
            GameHeartServices.GraphicsDeviceManager.DeviceReset -= GraphicsDeviceManagerOnDeviceReset;
            
            DeviceReset = null;
        }

        public void ToggleFullScreen()
        {
            if (!GameHeartServices.IsServicesLoaded)
                return;
            
            GameHeartServices.GraphicsDeviceManager.ToggleFullScreen();
        }

        public void ApplyChanges()
        {
            if (!GameHeartServices.IsServicesLoaded)
                return;
            
            GameHeartServices.GraphicsDeviceManager.ApplyChanges();
        }
        
        private void GameHeartServicesOnLoaded()
        {
            GameHeartServices.Loaded -= GameHeartServicesOnLoaded;
            GameHeartServices.GraphicsDeviceManager.DeviceReset += GraphicsDeviceManagerOnDeviceReset;
        }

        private void GraphicsDeviceManagerOnDeviceReset(object sender, EventArgs e)
        {
            DeviceReset?.Invoke();
        }
    }
}