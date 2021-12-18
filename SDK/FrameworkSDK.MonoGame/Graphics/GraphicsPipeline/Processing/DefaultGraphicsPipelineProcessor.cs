using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    [UsedImplicitly]
    internal class DefaultGraphicsPipelineProcessor : IGraphicsPipelineProcessor
    {
        private Guid _id = Guid.NewGuid();
        private IGraphicsPipelinePassAssociateService AssociateService { get; }

        private IReadOnlyObservableList<IGraphicComponent> _currentComponents;
        private bool _disposed;
        
        private readonly Dictionary<string, List<IGraphicComponent>> _componentsByPassesMap = new Dictionary<string, List<IGraphicComponent>>();
        
        public DefaultGraphicsPipelineProcessor([NotNull] IGraphicsPipelinePassAssociateService associateService)
        {
            AssociateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
        }
        
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DefaultGraphicsPipelineProcessor));
            _disposed = true;
            
            UnsubscribeToComponentsChanges(_currentComponents);
            _currentComponents = null;
            
            _componentsByPassesMap.Clear();
        }

        public void SetupComponents(IReadOnlyObservableList<IGraphicComponent> componentsCollection)
        {
            if (componentsCollection == null) throw new ArgumentNullException(nameof(componentsCollection));
            if (_disposed) throw new ObjectDisposedException(nameof(DefaultGraphicsPipelineProcessor));

            if (_currentComponents != null)
            {
                UnsubscribeToComponentsChanges(_currentComponents);
            }

            _currentComponents = componentsCollection;
            SubscribeToComponentsChanges(_currentComponents);
            
            RebuildMap();
        }

        public void Process(GameTime gameTime, IPipelineContext context, IGraphicsPipelineAction pipelineAction)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DefaultGraphicsPipelineProcessor));
            
            if (pipelineAction.IsDisabled) return;

            var passName = pipelineAction.Name;
            var targetComponents = _componentsByPassesMap.ContainsKey(passName)
                ? (IReadOnlyList<IGraphicComponent>)_componentsByPassesMap[passName]
                : Array.Empty<IGraphicComponent>(); 
            
            pipelineAction.Process(context, gameTime, targetComponents);
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
            var targetPass = AssociateService.GetAssociatedPass(component);
            AddToMap(component, targetPass);
        }
        
        private void ComponentsOnItemRemoved(IGraphicComponent component)
        {
            var targetPass = AssociateService.GetAssociatedPass(component);
            RemoveFromMap(component, targetPass);
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

        private void RebuildMap()
        {
            _componentsByPassesMap.Clear();

            foreach (var component in _currentComponents)
            {
                var targetPass = AssociateService.GetAssociatedPass(component);
                AddToMap(component, targetPass);
            }
        }
    }
}