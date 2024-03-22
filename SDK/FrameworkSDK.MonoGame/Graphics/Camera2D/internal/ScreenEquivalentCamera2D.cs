using System;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    internal class ScreenEquivalentCamera2D : ICamera2D, IDisposable
    {
        private IDisplayService DisplayService { get; }

        private Vector2 _size;
        private bool _initialized;

        internal ScreenEquivalentCamera2D([NotNull] IDisplayService displayService)
        {
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            
            DisplayService.DeviceReset += DisplayServiceOnDeviceReset;
        }
        
        public void Dispose()
        {
            DisplayService.DeviceReset -= DisplayServiceOnDeviceReset;
        }

        private void DisplayServiceOnDeviceReset()
        {
            _initialized = true;
            _size = new Vector2(DisplayService.PreferredBackBufferWidth, DisplayService.PreferredBackBufferHeight);
        }

        public Vector2 GetPosition()
        {
            return Vector2.Zero;
        }

        public Vector2 GetSize()
        {
            if (!_initialized)
            {
                _initialized = true;
                _size = new Vector2(DisplayService.PreferredBackBufferWidth, DisplayService.PreferredBackBufferHeight);
            }
            return _size;
        }

        public Rectangle ToDisplay(RectangleF worldRectangle)
        {
            return worldRectangle.ToRectangle();
        }

        public Vector2 ToDisplay(Vector2 worldPoint)
        {
            return worldPoint;
        }

        public RectangleF FromDisplay(Rectangle displayRectangle)
        {
            return displayRectangle.ToRectangleF();
        }

        public Vector2 FromDisplay(Vector2 displayPoint)
        {
            return displayPoint;
        }
    }
}