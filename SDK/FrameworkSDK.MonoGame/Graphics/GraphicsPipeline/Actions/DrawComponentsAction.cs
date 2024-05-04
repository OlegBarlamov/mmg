using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawComponentsAction : DrawComponentsInCameraAction
    {
        public DrawComponentsAction([NotNull] string name) : base(name, CameraProvider)
        {
        }

        private static ICamera2D CameraProvider(IGraphicDeviceContext context)
        {
            return context.Camera2DService.GetActiveCamera();
        }
    }
}