using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        public static IGraphicsPipeline Empty { get; } = new EmptyGraphicsPipeline();

        public IReadOnlyList<IGraphicsPipelineAction> Actions { get; }
        private IReadOnlyObservableList<IGraphicComponent> GraphicComponents { get; }
        private IGraphicsPipelinePassAssociateService PipelinePassAssociateService { get; }

        private readonly ModuleLogger _logger;

        private readonly Dictionary<string, List<IGraphicComponent>> _componentsByPassesMap = new Dictionary<string, List<IGraphicComponent>>();
        private bool _disposed;

        public GraphicsPipeline(
            [NotNull] IReadOnlyList<IGraphicsPipelineAction> actions,
            [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents,
            [NotNull] IGraphicsPipelinePassAssociateService pipelinePassAssociateService,
            IFrameworkLogger frameworkLogger)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            GraphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
            PipelinePassAssociateService = pipelinePassAssociateService ?? throw new ArgumentNullException(nameof(pipelinePassAssociateService));
            _logger = new ModuleLogger(frameworkLogger, LogCategories.Rendering);
            
            SubscribeToComponentsChanges(GraphicComponents);
            BuildMap();
        }

        private void SubscribeToComponentsChanges(IReadOnlyObservableList<IGraphicComponent> components)
        {
            components.ItemAdded += ComponentsOnItemAdded;
            components.ItemRemoved += ComponentsOnItemRemoved;
        }

        private void UnsubscribeToComponentsChanges(IReadOnlyObservableList<IGraphicComponent> components)
        {
            components.ItemAdded -= ComponentsOnItemAdded;
            components.ItemRemoved -= ComponentsOnItemRemoved;
        }
        
        private void ComponentsOnItemAdded(IGraphicComponent component)
        {
            var targetPasses = PipelinePassAssociateService.GetAssociatedPasses(component);
            foreach (var targetPass in targetPasses)
            {
                AddToMap(component, targetPass);
            }
        }
        
        private void ComponentsOnItemRemoved(IGraphicComponent component)
        {
            var targetPasses = PipelinePassAssociateService.GetAssociatedPasses(component);
            foreach (var targetPass in targetPasses)
            {
                RemoveFromMap(component, targetPass);
            }
        }
        
        private void BuildMap()
        {
            foreach (var component in GraphicComponents)
            {
                ComponentsOnItemAdded(component);
            }
        }
        
        private void AddToMap(IGraphicComponent component, string targetPass)
        {
            if (string.IsNullOrWhiteSpace(targetPass))
                return;

            if (!_componentsByPassesMap.ContainsKey(targetPass))
            {
                var list = new List<IGraphicComponent>
                {
                    component
                };
                _componentsByPassesMap.Add(targetPass, list);
            }
            else
            {
                _componentsByPassesMap[targetPass].Add(component);
            }
        }

        private void RemoveFromMap(IGraphicComponent component, string targetPass)
        {
            if (string.IsNullOrWhiteSpace(targetPass))
                return;
            
            if (!_componentsByPassesMap.ContainsKey(targetPass))
                return;

            _componentsByPassesMap[targetPass].Remove(component);
        }
        
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GraphicsPipeline));
            _disposed = true;
            
            UnsubscribeToComponentsChanges(GraphicComponents);

            _componentsByPassesMap.Clear();

            foreach (var pipelineAction in Actions)
            {
                try
                {
                    pipelineAction.Dispose();
                }
                catch (Exception e)
                {
                    _logger.Error($"Graphics pipeline action {pipelineAction.Name} dispose error", e);
                }
            }
            
            _logger.Dispose();
        }

        private IGraphicsPipelineAction _iterableAction;
        private int _iterableIndex;
        private List<IGraphicComponent> _passComponentsArray;
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            for (_iterableIndex = 0; _iterableIndex < Actions.Count; _iterableIndex++)
            {
                _iterableAction = Actions[_iterableIndex];
                if (_iterableAction.IsDisabled)
                    continue;

                if (_componentsByPassesMap.TryGetValue(_iterableAction.Name, out _passComponentsArray))
                {
                    _iterableAction.Process(gameTime, graphicDeviceContext, _passComponentsArray);
                }
                else
                {
                    _iterableAction.Process(gameTime, graphicDeviceContext, Array.Empty<IGraphicComponent>());
                }

            }
        }
    }
}