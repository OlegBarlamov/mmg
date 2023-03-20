using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public static class GraphicsPipelineBuilderPresetsExtensions
    {
        public static GraphicsPipeline2DDrawingPreset Drawing2DPreset([NotNull] this IGraphicsPipelineBuilder builder, BeginDrawConfig beginDrawConfig = null)
        {
            return new GraphicsPipeline2DDrawingPreset(builder, beginDrawConfig ?? new BeginDrawConfig());
        }
    }
}