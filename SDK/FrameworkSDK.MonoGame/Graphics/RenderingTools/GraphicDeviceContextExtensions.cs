using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public static class GraphicDeviceContextExtensions
    {
        public static void BeginDraw(this IGraphicDeviceContext context, BeginDrawConfig config)
        {
            context.BeginDraw(config.SortMode, config.BlendState, config.SamplerState, config.DepthStencilState, config.RasterizerState, config.Effect, config.TransformMatrix);
        }
    }
}