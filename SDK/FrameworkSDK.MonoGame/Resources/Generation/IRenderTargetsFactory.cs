using FrameworkSDK.MonoGame.Graphics;
using Microsoft.Xna.Framework.Graphics;

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

        RenderTarget2D CreateRenderTarget(
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared);

        RenderTarget2D CreateRenderTarget(
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage);

        RenderTarget2D CreateRenderTarget(
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat);

        RenderTarget2D CreateRenderTarget(int width, int height);
        
        IRenderTargetWrapper CreateFullScreenRenderTarget(
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared,
            int arraySize);
        
        IRenderTargetWrapper CreateFullScreenRenderTarget(
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared);
        
        IRenderTargetWrapper CreateFullScreenRenderTarget(
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage);
        
        IRenderTargetWrapper CreateFullScreenRenderTarget(
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat);
        
        IRenderTargetWrapper CreateFullScreenRenderTarget();
    }
}