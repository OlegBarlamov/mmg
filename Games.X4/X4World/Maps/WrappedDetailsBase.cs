using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace X4World
{
    public abstract class WrappedDetailsBase<TAggregatedData> : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; protected set; }
        public string LayerName { get; protected set; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; protected set; }
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; protected set; }

        public TAggregatedData AggregatedData { get; }

        protected WrappedDetailsBase(
            IWrappedDetails parent,
            Vector3 localPosition,
            TAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public Vector3 GetWorldPosition()
        {
            return Parent.GetWorldPosition() + Position;
        }

        public virtual void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects)
        {
            IsDetailsGenerated = true;
            foreach (var wrappedDetail in objects)
            {
                Details.Add(wrappedDetail);
            }
        }
    }
}
