using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public static class GraphicsPipelineBuilderExtensions
    {
        public static IGraphicsPipelineBuilder Do(this IGraphicsPipelineBuilder builder, GraphicsActionDelegate action, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = GenerateActionName();
            return builder.AddAction(new GraphicsPipelineActionDelegate(name, action));
        }
        
        public static IGraphicsPipelineBuilder Do(this IGraphicsPipelineBuilder builder, Action<IGraphicDeviceContext> action, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = GenerateActionName();
            return builder.AddAction(new GraphicsPipelineActionDelegate(name, (time, context, components) => action(context)));
        }
        
        public static IGraphicsPipelineBuilder BeginDraw([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] BeginDrawConfig beginDrawConfig)
        {
            return builder.AddAction(new BeginDrawAction(GenerateActionName(nameof(BeginDraw)), beginDrawConfig));
        }

        public static IGraphicsPipelineBuilder EndDraw([NotNull] this IGraphicsPipelineBuilder builder)
        {
            return builder.Do(context => context.EndDraw(), GenerateActionName(nameof(EndDraw)));
        }

        public static IGraphicsPipelineBuilder Clear([NotNull] this IGraphicsPipelineBuilder builder, Color clearColor)
        {
            return builder.Do(context => context.Clear(clearColor), GenerateActionName(nameof(Clear)));
        }
        
        public static IGraphicsPipelineBuilder DrawComponents([NotNull] this IGraphicsPipelineBuilder builder, [NotNull] string name = View.DefaultViewPassName)
        {
            return builder.AddAction(new DrawComponentsAction(name));
        }

        public static IGraphicsPipelineBuilder DrawComponents([NotNull] this IGraphicsPipelineBuilder builder,
            ICamera2D camera2D, [NotNull] string name = View.DefaultViewPassName)
        {
            return builder.AddAction(new DrawComponentsInCameraAction(name, context => camera2D));
        }
        
        public static IGraphicsPipelineBuilder DrawComponents([NotNull] this IGraphicsPipelineBuilder builder,
            Func<IGraphicDeviceContext, ICamera2D> camera2DProvider, [NotNull] string name = View.DefaultViewPassName)
        {
            return builder.AddAction(new DrawComponentsInCameraAction(name, camera2DProvider));
        }

        public static IGraphicsPipelineBuilder SetRenderTarget([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] RenderTarget2D renderTarget)
        {
            return builder.AddAction(new SetRenderTargetAction(GenerateActionName(nameof(SetRenderTarget)), renderTarget));
        }
        
        public static IGraphicsPipelineBuilder SetRenderTargetToDisplay([NotNull] this IGraphicsPipelineBuilder builder)
        {
            return builder.AddAction(new SetRenderTargetToDisplayAction(GenerateActionName(nameof(SetRenderTargetToDisplay))));
        }

        public static IGraphicsPipelineBuilder DrawRenderTargetToDisplay([NotNull] this IGraphicsPipelineBuilder builder, RenderTarget2D renderTarget2D)
        {
            return builder.AddAction(new DrawRenderTargetToDisplay(
                GenerateActionName(nameof(DrawRenderTargetToDisplay)), new BeginDrawConfig(), renderTarget2D));
        }
        
        public static IGraphicsPipelineBuilder DrawRenderTargetToDisplay([NotNull] this IGraphicsPipelineBuilder builder, BeginDrawConfig beginDrawConfig, RenderTarget2D renderTarget2D)
        {
            return builder.AddAction(new DrawRenderTargetToDisplay(
                GenerateActionName(nameof(DrawRenderTargetToDisplay)), beginDrawConfig, renderTarget2D));
        }

        public static IGraphicsPipelineBuilder SimpleRender<TVertexType>([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Effect effect, [NotNull] VertexBuffer vertexBuffer, [NotNull] IndexBuffer indexBuffer, [NotNull] string name = View.DefaultViewPassName)
            where TVertexType : struct, IVertexType
        {
            return builder.AddAction(
                new SimpleRenderComponentsMeshes<TVertexType>(name, effect, vertexBuffer, indexBuffer));
        }
        
        public static IGraphicsPipelineBuilder RenderGrouped<TVertexType>([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Effect effect, [NotNull] VertexBuffer vertexBuffer, [NotNull] IndexBuffer indexBuffer, [NotNull] string name = View.DefaultViewPassName)
            where TVertexType : struct, IVertexType
        {
            return builder.AddAction(
                new SimpleRenderInstancedMeshes<TVertexType>(name, effect, vertexBuffer, indexBuffer));
        }

        public static IGraphicsPipelineBuilder ApplyActiveCameraToShader([NotNull] this IGraphicsPipelineBuilder builder, IEffectMatrices effect)
        {
            return builder.AddAction(
                new ApplyActiveCameraToShaderAction(GenerateActionName(nameof(ApplyActiveCameraToShaderAction)), effect));
        }

        public static IGraphicsPipelineBuilder SetBlendState([NotNull] this IGraphicsPipelineBuilder builder, BlendState blendState)
        {
            return builder.Do(context =>
            {
                context.SetBlendState(blendState);
            }, GenerateActionName(nameof(SetBlendState)));
        }
        
        public static IGraphicsPipelineBuilder SetDepthStencilState([NotNull] this IGraphicsPipelineBuilder builder, DepthStencilState depthStencilState)
        {
            return builder.Do(context =>
            {
                context.SetDepthStencilState(depthStencilState);
            }, GenerateActionName(nameof(SetDepthStencilState)));
        }
        
        public static IGraphicsPipelineBuilder SetRasterizerState([NotNull] this IGraphicsPipelineBuilder builder, RasterizerState rasterizerState)
        {
            return builder.Do(context =>
            {
                context.SetRasterizerState(rasterizerState);
            }, GenerateActionName(nameof(SetRasterizerState)));
        }

        public static IGraphicsPipelineBuilder SetRenderingConfigs([NotNull] this IGraphicsPipelineBuilder builder, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            return builder.Do(context =>
            {
                context.SetDepthStencilState(depthStencilState);
                context.SetBlendState(blendState);
                context.SetRasterizerState(rasterizerState);
            }, GenerateActionName(nameof(SetRenderingConfigs)));
        }

        internal static string GenerateActionName(string prefix = "")
        {
            return NamesGenerator.Hash(HashType.Number, prefix);
        }
    }
}