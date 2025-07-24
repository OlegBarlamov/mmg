using System;

namespace Epic.Data.GameResources
{
    internal interface IResourceByPlayerEntity
    {
        Guid ResourceId { get; }
        Guid PlayerId { get; }
        int Amount { get; }
    }

    internal class MutableResourceByPlayerEntity : IResourceByPlayerEntity
    {
        public Guid ResourceId { get; set; }
        public Guid PlayerId { get; set; }
        public int Amount { get; set; }
    }
}
