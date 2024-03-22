using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public static class GraphicsPipelineBuilderPresetsExtensions
    {
        public static GraphicsPipeline2DDrawingPreset Drawing2DPreset([NotNull] this IGraphicsPipelineBuilder builder, Color? clearColor = null, BeginDrawConfig beginDrawConfig = null)
        {
            return new GraphicsPipeline2DDrawingPreset(builder, beginDrawConfig ?? new BeginDrawConfig(), clearColor ?? Color.Black);
        }
    }
}