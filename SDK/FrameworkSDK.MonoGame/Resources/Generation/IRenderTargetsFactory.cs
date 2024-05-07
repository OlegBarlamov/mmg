using System;
using FrameworkSDK.MonoGame.Graphics;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    public interface IRenderTargetsFactory
    {
        RenderTarget2D CreateRenderTarget(
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared,
            int arraySize);

        RenderTarget2D CreateRenderTarget(int width, int height);
        
        IRenderTargetWrapper CreateDisplaySizedRenderTarget(
            Func<SizeInt, SizeInt> getSize,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared,
            int arraySize);
    }

    public static class RenderTargetsFactoryExtensions
    {
        public static RenderTarget2D CreateRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared)
        {
            return renderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat,
                preferredMultiSampleCount, usage, shared, 1);
        }

        public static RenderTarget2D CreateRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage)
        {
            return renderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat,
                preferredMultiSampleCount, usage, false, 1);
        }

        public static RenderTarget2D CreateRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
        {
            return renderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat,
                0, RenderTargetUsage.DiscardContents, false, 1);
        }

        public static IRenderTargetWrapper CreateDisplaySizedRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            Func<SizeInt, SizeInt> getSize,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared)
        {
            return renderTargetsFactory.CreateDisplaySizedRenderTarget(getSize, mipMap, preferredFormat,
                preferredDepthFormat, preferredMultiSampleCount, usage, shared, 1);
        }

        public static IRenderTargetWrapper CreateDisplaySizedRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            Func<SizeInt, SizeInt> getSize,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage)
        {
            return renderTargetsFactory.CreateDisplaySizedRenderTarget(getSize, mipMap, preferredFormat,
                preferredDepthFormat, preferredMultiSampleCount, usage, false, 1);
        }
        
        public static IRenderTargetWrapper CreateDisplaySizedRenderTarget(this IRenderTargetsFactory renderTargetsFactory,
            Func<SizeInt, SizeInt> getSize,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
            {
                return renderTargetsFactory.CreateDisplaySizedRenderTarget(getSize, mipMap, preferredFormat,
                    preferredDepthFormat, 0, RenderTargetUsage.DiscardContents, false, 1);
            }

        public static IRenderTargetWrapper CreateDisplaySizedRenderTarget(this IRenderTargetsFactory renderTargetsFactory, Func<SizeInt, SizeInt> getSize)
        {
            return renderTargetsFactory.CreateDisplaySizedRenderTarget(getSize, false, SurfaceFormat.Color, DepthFormat.None, 0,
                RenderTargetUsage.DiscardContents, false, 1);
        }
        
        public static IRenderTargetWrapper CreateDisplaySizedRenderTarget(this IRenderTargetsFactory renderTargetsFactory)
        {
            return renderTargetsFactory.CreateDisplaySizedRenderTarget(size => size);
        }
    }
}