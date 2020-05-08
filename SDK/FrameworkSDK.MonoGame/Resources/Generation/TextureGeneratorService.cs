using System;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    [UsedImplicitly]
    internal class TextureGeneratorService : ITextureGeneratorService
    {
        private IGameHeartServices GameHeartServices { get; }

        public TextureGeneratorService([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
        }

        public Texture2D DiffuseColor(Color color)
        {
            return GameHeartServices.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(color);
        }

        public Texture2D GradientColor(Color color1, Color color2, int width, int height, float angle,
            float offset = 0)
        {
            return GameHeartServices.GraphicsDeviceManager.GraphicsDevice.GetTextureGradientColor(color1, color2, width,
                height, angle, offset);
        }
    }
}