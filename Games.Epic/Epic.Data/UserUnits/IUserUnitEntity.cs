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
}