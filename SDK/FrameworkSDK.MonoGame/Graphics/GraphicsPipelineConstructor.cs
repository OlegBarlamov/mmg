using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Graphics
{
    public class GraphicsPipelineConstructor
    {
        private readonly List<GraphicPipelineStep> _steps;

        public GraphicsPipelineConstructor Pass(IGraphicsPass pass)
        {
            throw new NotImplementedException();
        }

        public IGraphicsPipeline Create()
        {
            return new GraphicsPipeline(_steps);
        }
    }
}
