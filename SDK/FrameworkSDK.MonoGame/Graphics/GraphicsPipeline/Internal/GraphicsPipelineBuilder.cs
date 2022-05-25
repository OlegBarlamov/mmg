using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipelineBuilder : IGraphicsPipelineBuilder
    {
        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private readonly IReadOnlyObservableList<IGraphicComponent> _graphicComponents;
        private readonly List<IGraphicsPipelineAction> _actions = new List<IGraphicsPipelineAction>();
        private bool _built;

        public GraphicsPipelineBuilder(
        [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents,
            [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService)
        {
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            _graphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
        }
        
        public IGraphicsPipelineBuilder AddAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            _actions.Add(action);
            return this;
        }

        public IGraphicsPipeline Build()
        {
            if (_built) throw new FrameworkMonoGameException(Strings.Exceptions.Graphics.PipelineAlreadyBuilt);
            _built = true;
            
            var actions = new List<IGraphicsPipelineAction>(_actions);
            _actions.Clear();
            
            return new GraphicsPipeline(actions, _graphicComponents, GraphicsPipelinePassAssociateService);
        }
    }
}