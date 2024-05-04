using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SetActiveCamera2D : GraphicsPipelineActionBase
    {
        public Func<ICamera2D> CameraProvider { get; }

        public SetActiveCamera2D([NotNull] string name, [NotNull] Func<ICamera2D> cameraProvider)
            : base(name)
        {
            CameraProvider = cameraProvider ?? throw new ArgumentNullException(nameof(cameraProvider));
        }

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.Camera2DService.SetActiveCamera(CameraProvider.Invoke());
        }
    }

    public static class SetActiveCamera2DBuilderExtension
    {
        public static IGraphicsPipelineBuilder SetActiveCamera2D([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Func<ICamera2D> cameraProvider)
        {
            return builder.AddAction(new SetActiveCamera2D(
                GraphicsPipelineBuilderExtensions.GenerateActionName(nameof(SetActiveCamera2D)), cameraProvider));
        }
    }
}