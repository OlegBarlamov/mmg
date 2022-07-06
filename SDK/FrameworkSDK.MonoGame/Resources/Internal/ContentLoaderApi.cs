using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal class ContentLoaderApi : IContentLoaderApi
    {
        private IContentContainer Container { get; }
        private ITextureGeneratorService TextureGeneratorInternal { get; }
        private IRenderTargetsFactory RenderTargetsFactory { get; }

        public ContentLoaderApi([NotNull] IContentContainer container, [NotNull] ITextureGeneratorService textureGeneratorInternal,
            [NotNull] IRenderTargetsFactory renderTargetsFactory)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            TextureGeneratorInternal = textureGeneratorInternal ?? throw new ArgumentNullException(nameof(textureGeneratorInternal));
            RenderTargetsFactory = renderTargetsFactory ?? throw new ArgumentNullException(nameof(renderTargetsFactory));
        }
        
        public T Load<T>(string assetName)
        {
            return Container.Load<T>(assetName);
        }

        public Texture2D DiffuseColor(Color color)
        {
            var result = TextureGeneratorInternal.DiffuseColor(color);
            Container.AddResource(result);
            return result;
        }

        public Texture2D GradientColor(Color color1, Color color2, int width, int height, float angle,
            float offset = 0)
        {
            var result = TextureGeneratorInternal.GradientColor(color1, color2, width, height, angle, offset);
            Container.AddResource(result);
            return result;
        }

        public Texture2D HeightMap(int[,] heights, int minValue, int maxValue, Color minValueColor, Color maxValueColor)
        {
            return TextureGeneratorInternal.HeightMap(heights, minValue, maxValue, minValueColor, maxValueColor);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            var result = RenderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat,
                preferredMultiSampleCount, usage, shared, arraySize);
            Container.AddResource(result);
            return result;
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
        {
            var result = RenderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat,
                preferredMultiSampleCount, usage, shared);
            Container.AddResource(result);
            return result;
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            var result = RenderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat,
                preferredDepthFormat, preferredMultiSampleCount, usage);
            Container.AddResource(result);
            return result;
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
        {
            var result = RenderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat,
                preferredDepthFormat);
            Container.AddResource(result);
            return result;
        }

        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            var result = RenderTargetsFactory.CreateRenderTarget(width, height); 
            Container.AddResource(result);
            return result;
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            var result = RenderTargetsFactory.CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared, arraySize);
            Container.AddResource(result);
            return result;
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
        {
            var result = RenderTargetsFactory.CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared);
            Container.AddResource(result);
            return result;
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            var result = RenderTargetsFactory.CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
            Container.AddResource(result);
            return result;
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
        {
            var result = RenderTargetsFactory.CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat);
            Container.AddResource(result);
            return result;
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget()
        {
            var result = RenderTargetsFactory.CreateFullScreenRenderTarget();
            Container.AddResource(result);
            return result;
        }
    }
}