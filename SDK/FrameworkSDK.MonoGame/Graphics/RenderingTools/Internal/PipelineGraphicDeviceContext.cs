using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools.Internal;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    internal class PipelineGraphicDeviceContext : IGraphicDeviceContext
    {
        public IDrawContext DrawContext => _drawContext;
        private readonly CameraBasedDrawContext _drawContext;
        public IRenderContext RenderContext { get; }
        public IGeometryRenderer GeometryRenderer { get; }
        public ICamera2DService Camera2DService { get; }
        public ICamera3DService Camera3DService { get; }
        public IDisplayService DisplayService { get; }
        public IDebugInfoService DebugInfoService { get; }
        private IIndicesBuffersFiller IndicesBuffersFiller { get; }
        private SpriteBatch SpriteBatch { get; }
        private GraphicsDevice GraphicsDevice { get; }

        public void Dispose()
        {
            DrawContext.Dispose();
            RenderContext.Dispose();
        }

        public PipelineGraphicDeviceContext(
            [NotNull] SpriteBatch spriteBatch,
            [NotNull] GraphicsDevice graphicsDevice,
            [NotNull] IDisplayService displayService,
            [NotNull] ICamera3DService camera3DService,
            [NotNull] IDebugInfoService debugInfoService,
            [NotNull] IIndicesBuffersFiller indicesBuffersFiller,
            [NotNull] ICamera2DService camera2DService)
        {
            SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            IndicesBuffersFiller = indicesBuffersFiller ?? throw new ArgumentNullException(nameof(indicesBuffersFiller));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));

            _drawContext = new CameraBasedDrawContext(new SpriteBatchDrawContext(SpriteBatch), Camera2DService);
            RenderContext = new RenderContext(GraphicsDevice, IndicesBuffersFiller);
            
            GeometryRenderer = new GeometryRenderer(RenderContext);
        }

        public void BeginDraw(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            SpriteBatch.Begin(sortMode,blendState,samplerState,depthStencilState,rasterizerState,effect,transformMatrix);
        }
        
        public void DrawInCamera(ICamera2D camera, Action<IDrawContext> drawAction)
        {
            _drawContext.CameraOverride = camera;
            try
            {
                drawAction.Invoke(_drawContext);
            }
            finally
            {
                _drawContext.CameraOverride = null;
            }
        }

        public void EndDraw()
        {
            SpriteBatch.End();
        }

        public void SetRenderTarget(RenderTarget2D renderTarget2D)
        {
            if (renderTarget2D == null) throw new ArgumentNullException(nameof(renderTarget2D));
            
            GraphicsDevice.SetRenderTarget(renderTarget2D);
        }

        public void SetRenderTargetToDisplay()
        {
            GraphicsDevice.SetRenderTarget(null);
        }

        public void Clear(Color color)
        {
            GraphicsDevice.Clear(color);
        }

        public void SetBlendState(BlendState blendState)
        {
            GraphicsDevice.BlendState = blendState;
        }

        public void SetDepthStencilState(DepthStencilState depthStencilState)
        {
            GraphicsDevice.DepthStencilState = depthStencilState;
        }

        public void SetRasterizerState(RasterizerState rasterizerState)
        {
            GraphicsDevice.RasterizerState = rasterizerState;
        }
    }
}
