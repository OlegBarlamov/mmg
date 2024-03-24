using System;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Resources.Generation.Primitives;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    internal class TexturePrimitivesGenerator : ITexturePrimitivesGenerator
    {
        private IGameHeartServices GameHeartServices { get; }

        public TexturePrimitivesGenerator([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
        }
        
        public Texture2D Circle(int radius, Color fillColor, Color? strokeColor, int? strokeThickness, Color? outerColor)
        {
            return GameHeartServices.GraphicsDeviceManager.GraphicsDevice.GenerateCircle(radius, fillColor, strokeColor, strokeThickness, outerColor);
        }
    }
}