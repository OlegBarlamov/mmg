using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    internal class TextureGeneratorServicePublic : ITextureGeneratorService
    {
        private ITextureGeneratorApi TextureGeneratorApi { get; }
        private IResourceReferencesService ResourceReferencesService { get; }

        public TextureGeneratorServicePublic([NotNull] ITextureGeneratorApi textureGeneratorApi,
            [NotNull] IResourceReferencesService resourceReferencesService)
        {
            TextureGeneratorApi = textureGeneratorApi ?? throw new ArgumentNullException(nameof(textureGeneratorApi));
            ResourceReferencesService = resourceReferencesService ?? throw new ArgumentNullException(nameof(resourceReferencesService));
        }

        public Texture2D EmptyTexture(int width, int height)
        {
            var texture = TextureGeneratorApi.EmptyTexture(width, height);
            CountTexture(texture);
            return texture;
        }

        public Texture2D DiffuseColor(Color color)
        {
            var texture = TextureGeneratorApi.DiffuseColor(color);
            CountTexture(texture);
            return texture;
        }

        public Texture2D GradientColor(Color color1, Color color2, int width, int height, float angleDegrees, float offset = 0)
        {
            var texture = TextureGeneratorApi.GradientColor(color1, color2, width, height, angleDegrees, offset);
            CountTexture(texture);
            return texture;
        }

        public Texture2D HeightMap(int[,] heights, int minValue, int maxValue, Color minValueColor, Color maxValueColor)
        {
            var texture = TextureGeneratorApi.HeightMap(heights, minValue, maxValue, minValueColor, maxValueColor);
            CountTexture(texture);
            return texture;
        }
        
        private void CountTexture(Texture2D texture)
        {
            ResourceReferencesService.AddPackageless(texture);
            texture.Disposing += TextureOnDisposing;
        }

        private void TextureOnDisposing(object sender, EventArgs e)
        {
            var texture = (Texture2D) sender;
            texture.Disposing -= TextureOnDisposing;
            ResourceReferencesService.RemovePackageless(texture);
        }
    }
}