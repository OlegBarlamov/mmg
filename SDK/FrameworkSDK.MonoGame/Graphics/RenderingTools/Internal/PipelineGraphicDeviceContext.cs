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
        public ICamera2DProvider Camera2DProvider { get; }
        public ICamera3DProvider Camera3DProvider { get; }
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

        public PipelineGraphicDeviceContext([NotNull] SpriteBatch spriteBatch, [NotNull] GraphicsDevice graphicsDevice,
            [NotNull] IDisplayService displayService, [NotNull] ICamera3DProvider camera3DProvider,
            [NotNull] IDebugInfoService debugInfoService, [NotNull] IIndicesBuffersFiller indicesBuffersFiller,
            [NotNull] ICamera2DProvider camera2DProvider) : base()
        {
            SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            IndicesBuffersFiller = indicesBuffersFiller ?? throw new ArgumentNullException(nameof(indicesBuffersFiller));
            Camera2DProvider = camera2DProvider ?? throw new ArgumentNullException(nameof(camera2DProvider));

            _drawContext = new CameraBasedDrawContext(new SpriteBatchDrawContext(SpriteBatch), Camera2DProvider);
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
