using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    internal class DefaultGraphicsPipelineProcessorFactory : IGraphicsPipelineProcessorsFactory
    {
        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }

        public DefaultGraphicsPipelineProcessorFactory([NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService)
        {
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
        }
        
        public IGraphicsPipelineProcessor Create()
        {
            return new DefaultGraphicsPipelineProcessor(GraphicsPipelinePassAssociateService);
        }
    }
}