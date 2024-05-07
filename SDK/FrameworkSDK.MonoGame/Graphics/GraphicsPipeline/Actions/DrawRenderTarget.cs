using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawRenderTarget : GraphicsPipelineActionBase
    {
        public IRenderTargetWrapper RenderTargetWrapper { get; }
        public Func<Vector2> DisplayPositionProvider { get; }
        public Color Color { get; }

        public DrawRenderTarget([NotNull] string name, [NotNull] IRenderTargetWrapper renderTargetWrapper, [NotNull] Func<Vector2> displayPositionProvider, Color color) : base(name)
        {
            RenderTargetWrapper = renderTargetWrapper ?? throw new ArgumentNullException(nameof(renderTargetWrapper));
            DisplayPositionProvider = displayPositionProvider ?? throw new ArgumentNullException(nameof(displayPositionProvider));
            Color = color;
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.DrawContext.Draw(RenderTargetWrapper.RenderTarget, DisplayPositionProvider.Invoke(), Color);
        }
    }

    public static class DrawRenderTargetBuilderExtension
    {
        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder, IRenderTargetWrapper renderTarget)
        {
            return builder.DrawRenderTarget(renderTarget, () => Vector2.Zero, Color.White);
        }

        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            IRenderTargetWrapper renderTarget, [NotNull] Func<Vector2> displayPositionProvider)
        {
            return builder.DrawRenderTarget(renderTarget, displayPositionProvider, Color.White);
        }
        
        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder, IRenderTargetWrapper renderTarget, [NotNull] Func<Vector2> displayPositionProvider, Color color)
        {
            return builder.AddAction(new DrawRenderTarget(
                GraphicsPipelineBuilderExtensions.GenerateActionName(nameof(DrawRenderTarget)), renderTarget, displayPositionProvider, color));
        }
    }
}