using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    internal class ComponentsByPassAggregator : IComponentsByPassAggregator
    {
        public IReadOnlyDictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>> Aggregate(
            IReadOnlyList<IGraphicsPass> passes, IReadOnlyCollection<IGraphicComponent> components)
        {
            var result = new Dictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>>();
            var passesByName = new Dictionary<string, IGraphicsPass>();
            
            foreach (var pass in passes)
            {
                result.Add(pass, new List<IGraphicComponent>());
                passesByName.Add(pass.Name, pass);
            }

            foreach (var component in components)
            {
                var targetPassName = component.GraphicsPassName;

                if (!passesByName.TryGetValue(targetPassName, out var targetPass))
                    continue;

                if (!result.ContainsKey(targetPass))
                    result.Add(targetPass, new List<IGraphicComponent>());

                ((IList<IGraphicComponent>)result[targetPass]).Add(component);
            }

            return new ReadOnlyDictionary<IGraphicsPass, IReadOnlyCollection<IGraphicComponent>>(result);
        }
    }
}
