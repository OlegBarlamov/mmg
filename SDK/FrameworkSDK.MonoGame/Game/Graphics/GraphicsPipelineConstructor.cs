using System.Collections.Generic;

namespace FrameworkSDK.Game.Graphics
{
    public class GraphicsPipelineConstructor
    {
        private readonly List<GraphicPipelineStep> _steps;

        public GraphicsPipelineConstructor Pass(IGraphicsPass pass)
        {

        }

        public IGraphicsPipeline Create()
        {
            return new GraphicsPipeline(_steps);
        }
    }
}
