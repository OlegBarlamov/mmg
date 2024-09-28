using System;

namespace Epic.Core.Objects
{
    public interface IUserObject
    {
        Guid Id { get; }
        string Name { get; }
        UserObjectType Type { get; }
        string Hash { get; }
        bool IsBlocked { get; }
    }
}