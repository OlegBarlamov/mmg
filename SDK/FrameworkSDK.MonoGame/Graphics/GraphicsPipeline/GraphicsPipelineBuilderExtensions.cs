using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public static class GraphicsPipelineBuilderExtensions
    {
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] DrawPipelineAction drawAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (drawAction == null) throw new ArgumentNullException(nameof(drawAction));
            return builder.AddAction(drawAction);
        }

        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] RenderPipelineAction renderAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (renderAction == null) throw new ArgumentNullException(nameof(renderAction));
            return  builder.AddAction(renderAction);
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] SystemPipelineAction systemAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (systemAction == null) throw new ArgumentNullException(nameof(systemAction));
            return builder.AddAction(systemAction);
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IDrawContext> drawAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddAction(drawAction, builder.GetActionName(nameof(drawAction)));
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IRenderContext> renderAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddAction(renderAction, builder.GetActionName(nameof(renderAction)));
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IGraphicDeviceContext> systemAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddAction(systemAction, builder.GetActionName(nameof(systemAction)));
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IDrawContext> drawAction, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (drawAction == null) throw new ArgumentNullException(nameof(drawAction));
            if (name == null) throw new ArgumentNullException(nameof(name));
            var action = new FixedDrawPipelineAction(name, builder.DrawContext, drawAction);
            return builder.AddAction(action);
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IRenderContext> renderAction, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (renderAction == null) throw new ArgumentNullException(nameof(renderAction));
            if (name == null) throw new ArgumentNullException(nameof(name));
            var action = new FixedRenderPipelineAction(name, builder.RenderContext, renderAction);
            return builder.AddAction(action);
        }
        
        public static IGraphicsPipelineBuilder AddAction([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Action<GameTime, IGraphicDeviceContext> systemAction, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (systemAction == null) throw new ArgumentNullException(nameof(systemAction));
            if (name == null) throw new ArgumentNullException(nameof(name));
            var action = new FixedSystemPipelineAction(name, builder.GraphicDeviceContext, systemAction);
            return builder.AddAction(action);
        }

        public static IGraphicsPipelineBuilder BeginDraw([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] BeginDrawConfig beginDrawConfig, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (beginDrawConfig == null) throw new ArgumentNullException(nameof(beginDrawConfig));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new BeginDrawPipelineAction(name, beginDrawConfig, builder.GraphicDeviceContext));
        }

        public static IGraphicsPipelineBuilder BeginDraw([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] BeginDrawConfig beginDrawConfig)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(BeginDraw));
            return builder.BeginDraw(beginDrawConfig, name);
        }
        
        public static IGraphicsPipelineBuilder EndDraw([NotNull] this IGraphicsPipelineBuilder builder, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new EndDrawPipelineAction(name, builder.GraphicDeviceContext));
        }
        
        public static IGraphicsPipelineBuilder EndDraw([NotNull] this IGraphicsPipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(EndDraw));
            return builder.EndDraw(name);
        }
        
        public static IGraphicsPipelineBuilder Clear([NotNull] this IGraphicsPipelineBuilder builder, Color clearColor, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new ClearPipelineAction(name, clearColor, builder.GraphicDeviceContext));
        }
        
        public static IGraphicsPipelineBuilder Clear([NotNull] this IGraphicsPipelineBuilder builder, Color clearColor)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(Clear));
            return builder.Clear(clearColor, name);
        }
        
        public static IGraphicsPipelineBuilder Draw([NotNull] this IGraphicsPipelineBuilder builder, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new DrawComponentsPipelineAction(name, builder.DrawContext));
        }
        
        public static IGraphicsPipelineBuilder Draw([NotNull] this IGraphicsPipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(Draw));
            return builder.Draw(name);
        }
        
        public static IGraphicsPipelineBuilder Render([NotNull] this IGraphicsPipelineBuilder builder, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new RenderComponentsPipelineAction(name, builder.RenderContext));
        }
        
        public static IGraphicsPipelineBuilder Render([NotNull] this IGraphicsPipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(Render));
            return builder.Render(name);
        }
        
        public static IGraphicsPipelineBuilder SwitchRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] RenderTarget2D renderTarget, Color? color, [NotNull] string name)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (renderTarget == null) throw new ArgumentNullException(nameof(renderTarget));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return builder.AddAction(new SwitchRenderTargetPipelineAction(name, renderTarget, color, builder.GraphicDeviceContext));
        }
        
        public static IGraphicsPipelineBuilder SwitchRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] RenderTarget2D renderTarget, Color? color)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(SwitchRenderTarget));
            return builder.SwitchRenderTarget(renderTarget, color, name);
        }
        
        public static IGraphicsPipelineBuilder SwitchRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] RenderTarget2D renderTarget)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var name = builder.GetActionName(nameof(SwitchRenderTarget));
            return builder.SwitchRenderTarget(renderTarget, null, name);
        }
        

        private static string GetActionName(this IGraphicsPipelineBuilder builder, string postfix = null)
        {
            postfix = postfix != null ? $": {postfix}" : string.Empty;
            return $"action {builder.Pipeline.ActionsCount + 1}" + postfix;
        }
    }
}