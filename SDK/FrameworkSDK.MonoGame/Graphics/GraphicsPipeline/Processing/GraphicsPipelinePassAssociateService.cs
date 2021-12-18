using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Services;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    [UsedImplicitly]
    internal class GraphicsPipelinePassAssociateService : IGraphicsPipelinePassAssociateService
    {
        public const string DefaultPassName = "default";
        private IAppDomainService DomainService { get; }

        private bool _initialized;
        private bool _disposed;
        private ConcurrentDictionary<Type, string> _bufferedMap;

        public GraphicsPipelinePassAssociateService([NotNull] IAppDomainService domainService)
        {
            DomainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        }
        
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GraphicsPipelinePassAssociateService));
            _disposed = true;
            
            _bufferedMap.Clear();
        }
        
        public void Initialize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GraphicsPipelinePassAssociateService));
            if (_initialized) throw new ObjectInitializedException(nameof(GraphicsPipelinePassAssociateService));
            
            var newMap = new Dictionary<Type, string>();
            var types = DomainService.GetAllTypes();
            foreach (var type in types)
            {
                if (!typeof(IGraphicComponent).IsAssignableFrom(type) || !IsTypeAllowed(type))
                    continue;

                var passName = GetPassNameFromComponentType(type);
                newMap.Add(type, passName);
            }

            _bufferedMap = new ConcurrentDictionary<Type, string>(newMap);
            _initialized = true;
        }

        public string GetAssociatedPass(IGraphicComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (_disposed) throw new ObjectDisposedException(nameof(GraphicsPipelinePassAssociateService));
            if (!_initialized) throw new ObjectNotInitializedException(nameof(GraphicsPipelinePassAssociateService));

            var componentType = component.GetType();
            if (_bufferedMap.TryGetValue(componentType, out var passName))
                return passName;

            passName = GetPassNameFromComponentType(componentType);
            AddToBuffer(componentType, passName);
            return passName;
        }

        private void AddToBuffer(Type componentType, string passName)
        {
            _bufferedMap.TryAdd(componentType, passName);
        }
        
        private string GetPassNameFromComponentType(Type componentType)
        {
            return componentType.GetCustomAttributes<RenderPassAttribute>().FirstOrDefault()?.PassName ??
                   DefaultPassName;
        }

        private static bool IsTypeAllowed(Type graphicComponentType)
        {
            return !graphicComponentType.IsAbstract && !graphicComponentType.IsInterface;
        }
    }
}