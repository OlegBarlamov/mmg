using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawComponentsInCameraAction : GraphicsPipelineActionBase
    {
        private Func<IGraphicDeviceContext, ICamera2D> CameraProvider { get; }

        public DrawComponentsInCameraAction([NotNull] string name, [NotNull] Func<IGraphicDeviceContext, ICamera2D> cameraProvider) : base(name)
        {
            CameraProvider = cameraProvider ?? throw new ArgumentNullException(nameof(cameraProvider));
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            for (int i = 0; i < associatedComponents.Count; i++)
            {
                var component = associatedComponents[i];
                if (IsComponentVisibleByActiveCamera(graphicDeviceContext, component))
                {
                    graphicDeviceContext.DrawInCamera(CameraProvider.Invoke(graphicDeviceContext), context => component.Draw(gameTime, context));
                    graphicDeviceContext.DebugInfoService.IncrementCounter(DebugInfoDrawComponents);
                }
            }
        }
    }
}