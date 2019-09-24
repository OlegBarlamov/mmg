using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    public interface IComponentsByPassAggregator
    {
        IReadOnlyDictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>> Aggregate(
            IReadOnlyList<IGraphicsPass> passes, IReadOnlyCollection<IGraphicComponent> components);
    }
}
