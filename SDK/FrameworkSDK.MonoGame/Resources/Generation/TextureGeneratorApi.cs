using System;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    [UsedImplicitly]
    internal class TextureGeneratorApi : ITextureGeneratorApi
    {
        public ITexturePrimitivesGenerator Primitives { get; }
        private IGameHeartServices GameHeartServices { get; }

        public TextureGeneratorApi([NotNull] IGameHeartServices gameHeartServices)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            Primitives = new TexturePrimitivesGenerator(gameHeartServices);
        }

        public Texture2D EmptyTexture(int width, int height)
        {
            return GameHeartServices.GraphicsDeviceManager.GraphicsDevice.GetEmptyTexture(width, height);
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

        public Texture2D HeightMap(int[,] heights, int minValue, int maxValue, Color minValueColor, Color maxValueColor)
        {
            var minValueColorVector = minValueColor.ToVector4();
            var maxValueColorVector = maxValueColor.ToVector4();
            var width = heights.GetLength(0);
            var height = heights.GetLength(1);

            var colors = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var value = heights[x, y];
                    var colorVector = (value - minValue) / (float)maxValue * (maxValueColorVector - minValueColorVector) +
                                      minValueColorVector;
                    colors[x, y] = Color.FromNonPremultiplied(colorVector);
                }
            }
            
            var texture = GameHeartServices.GraphicsDeviceManager.GraphicsDevice.GetEmptyTexture(width, height);
            texture.SetDataToTexture(colors);
            
            return texture;
        }
    }
}