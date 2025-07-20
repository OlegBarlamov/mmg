using System;
using Epic.Data;

namespace Epic.Core.Services.Users
{
    public class MutableUserObject : IUserObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public string Hash { get; set; }
        public bool IsBlocked { get; set; }
        public IUserEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
