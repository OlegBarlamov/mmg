using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawRenderTarget : GraphicsPipelineActionBase
    {
        public RenderTarget2D RenderTarget2D { get; }
        public Func<Vector2> DisplayPositionProvider { get; }
        public Color Color { get; }

        public DrawRenderTarget([NotNull] string name, [NotNull] RenderTarget2D renderTarget2D, [NotNull] Func<Vector2> displayPositionProvider, Color color) : base(name)
        {
            RenderTarget2D = renderTarget2D ?? throw new ArgumentNullException(nameof(renderTarget2D));
            DisplayPositionProvider = displayPositionProvider ?? throw new ArgumentNullException(nameof(displayPositionProvider));
            Color = color;
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.DrawContext.Draw(RenderTarget2D, DisplayPositionProvider.Invoke(), Color);
        }
    }

    public static class DrawRenderTargetBuilderExtension
    {
        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder, RenderTarget2D renderTarget2D)
        {
            return builder.DrawRenderTarget(renderTarget2D, () => Vector2.Zero, Color.White);
        }

        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            RenderTarget2D renderTarget2D, [NotNull] Func<Vector2> displayPositionProvider)
        {
            return builder.DrawRenderTarget(renderTarget2D, displayPositionProvider, Color.White);
        }
        
        public static IGraphicsPipelineBuilder DrawRenderTarget([NotNull] this IGraphicsPipelineBuilder builder, RenderTarget2D renderTarget2D, [NotNull] Func<Vector2> displayPositionProvider, Color color)
        {
            return builder.AddAction(new DrawRenderTarget(
                GraphicsPipelineBuilderExtensions.GenerateActionName(nameof(DrawRenderTarget)), renderTarget2D, displayPositionProvider, color));
        }
    }
}