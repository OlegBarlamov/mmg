using System;

namespace Epic.Data.UserUnits
{
    public interface IUserUnitEntity
    {
        Guid Id { get; }
        Guid TypeId { get; }
        int Count { get; }
        Guid UserId { get; }
        bool IsAlive { get; }
    }

    internal class UserUnitEntity : IUserUnitEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public int Count { get; set; }
        public Guid UserId { get; set; }
        public bool IsAlive { get; set; }
    }
}