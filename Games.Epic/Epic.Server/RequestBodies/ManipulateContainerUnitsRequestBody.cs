using System;

namespace Epic.Server.RequestBodies
{
    public class ManipulateContainerUnitsRequestBody
    {
        public Guid? ContainerId { get; set; }
        public int? Amount { get; set; }
        public int? SlotIndex { get; set; }
    }
}