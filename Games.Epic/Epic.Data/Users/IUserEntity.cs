using System;

namespace Epic.Data
{
    public interface IUserEntity
    {
        Guid Id { get; }
        string Name { get; }
        string Hash { get; }
        bool IsBlocked { get; }
        bool IsSystem { get; }
    }
    
    internal class UserEntity : IUserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSystem { get; set; }
    } 
}