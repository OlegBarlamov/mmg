using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    /// <summary>
    /// Graphics pipeline tool
    /// </summary>
    public interface IGraphicDeviceContext : IDisposable
    {
        IDrawContext DrawContext { get; }
        IRenderContext RenderContext { get; }
        ICamera2DProvider Camera2DProvider { get; }
        ICamera3DProvider Camera3DProvider { get; }
        IDisplayService DisplayService { get; }
        IDebugInfoService DebugInfoService { get; }

        void BeginDraw(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);

        void EndDraw();

        void SetRenderTarget([NotNull] RenderTarget2D renderTarget2D);
        void SetRenderTargetToDisplay();

        void Clear(Color color);

        void SetBlendState(BlendState blendState);

        void SetDepthStencilState(DepthStencilState depthStencilState);

        void SetRasterizerState(RasterizerState rasterizerState);

        void DrawInCamera(ICamera2D camera, Action<IDrawContext> drawAction);
    }
}
