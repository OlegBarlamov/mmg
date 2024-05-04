using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SetActiveCamera3D : GraphicsPipelineActionBase
    {
        public Func<ICamera3D> CameraProvider { get; }

        public SetActiveCamera3D([NotNull] string name, [NotNull] Func<ICamera3D> cameraProvider)
            : base(name)
        {
            CameraProvider = cameraProvider ?? throw new ArgumentNullException(nameof(cameraProvider));
        }

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.Camera3DService.SetActiveCamera(CameraProvider.Invoke());
        }
    }

    public static class SetActiveCamera3DBuilderExtension
    {
        public static IGraphicsPipelineBuilder SetActiveCamera3D([NotNull] this IGraphicsPipelineBuilder builder,
            [NotNull] Func<ICamera3D> cameraProvider)
        {
            return builder.AddAction(new SetActiveCamera3D(
                GraphicsPipelineBuilderExtensions.GenerateActionName(nameof(SetActiveCamera3D)), cameraProvider));
        }
    }
}