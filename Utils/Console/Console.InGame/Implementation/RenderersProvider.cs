using System;
using System.Collections.Generic;

namespace Console.InGame.Implementation
{
    internal class RenderersProvider
    {
        private readonly Dictionary<Type, IDataRenderer> _registeredRenderers = new Dictionary<Type, IDataRenderer>();

        public bool IsRegistered(Type type)
        {
            return _registeredRenderers.ContainsKey(type);
        }

        public IDataRenderer GetRenderer(Type type)
        {
            return _registeredRenderers[type];
        }

        public void Register(Type type, IDataRenderer renderer)
        {
            _registeredRenderers.Add(type, renderer);
        }
    }
}