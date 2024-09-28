using System;

namespace Epic.Core.Objects
{
    public class MutableUserObject : IUserObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserObjectType Type { get; set; }
        public string Hash { get; set; }
        public bool IsBlocked { get; set; }
    }
}