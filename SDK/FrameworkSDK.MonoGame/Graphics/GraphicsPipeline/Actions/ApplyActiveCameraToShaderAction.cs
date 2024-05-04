using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class ApplyActiveCameraToShaderAction : GraphicsPipelineActionBase
    {
        private IEffectMatrices Effect { get; }

        public ApplyActiveCameraToShaderAction([NotNull] string name, [NotNull] IEffectMatrices effect)
            : base(name)
        {
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            Effect.View = graphicDeviceContext.Camera3DService.GetActiveCamera().GetView();
            Effect.Projection = graphicDeviceContext.Camera3DService.GetActiveCamera().GetProjection();
        }
    }
}