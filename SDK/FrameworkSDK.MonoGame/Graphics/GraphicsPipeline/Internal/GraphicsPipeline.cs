using System;
using System.Collections.Generic;
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
        public static IGraphicsPipeline Empty { get; } = 
            new GraphicsPipeline(
                Array.Empty<IGraphicsPipelineAction>(),
                new ObservableList<IGraphicComponent>(),
                new EmptyGraphicsPipelinePassAssociateService());
        
        public IReadOnlyList<IGraphicsPipelineAction> Actions { get; }
        private IReadOnlyObservableList<IGraphicComponent> GraphicComponents { get; }
        private IGraphicsPipelinePassAssociateService PipelinePassAssociateService { get; }
        
        private readonly Dictionary<string, List<IGraphicComponent>> _componentsByPassesMap = new Dictionary<string, List<IGraphicComponent>>();
        private bool _disposed;

        public GraphicsPipeline(
            [NotNull] IReadOnlyList<IGraphicsPipelineAction> actions,
            [NotNull] IReadOnlyObservableList<IGraphicComponent> graphicComponents,
            [NotNull] IGraphicsPipelinePassAssociateService pipelinePassAssociateService)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            GraphicComponents = graphicComponents ?? throw new ArgumentNullException(nameof(graphicComponents));
            PipelinePassAssociateService = pipelinePassAssociateService ?? throw new ArgumentNullException(nameof(pipelinePassAssociateService));
            
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
            var targetPass = PipelinePassAssociateService.GetAssociatedPass(component);
            AddToMap(component, targetPass);
        }
        
        private void ComponentsOnItemRemoved(IGraphicComponent component)
        {
            var targetPass = PipelinePassAssociateService.GetAssociatedPass(component);
            RemoveFromMap(component, targetPass);
        }
        
        private void BuildMap()
        {
            foreach (var component in GraphicComponents)
            {
                var targetPass = PipelinePassAssociateService.GetAssociatedPass(component);
                AddToMap(component, targetPass);
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
        }

        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                if (!action.IsDisabled)
                {
                    var passName = action.Name;
                    var targetComponents = _componentsByPassesMap.ContainsKey(passName)
                        ? (IReadOnlyList<IGraphicComponent>)_componentsByPassesMap[passName]
                        : Array.Empty<IGraphicComponent>(); 
                    
                    action.Process(gameTime, graphicDeviceContext, targetComponents);
                }
            }
        }
    }
}