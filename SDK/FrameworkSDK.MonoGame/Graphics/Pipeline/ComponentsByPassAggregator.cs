using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    internal class ComponentsByPassAggregator : IComponentsByPassAggregator
    {
        public IReadOnlyDictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>> Aggregate(IReadOnlyList<IGraphicsPass> passes, IReadOnlyList<IGraphicComponent> components)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>> Aggregate(IReadOnlyList<IGraphicsPass> passes, IReadOnlyCollection<IGraphicComponent> components)
        {
            throw new NotImplementedException();
        }
    }
}
