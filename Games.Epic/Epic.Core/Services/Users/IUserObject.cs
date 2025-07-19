using System;
using Epic.Core.Services.Players;

namespace Epic.Core.Objects
{
    public interface IUserObject
    {
        Guid Id { get; }
        string Name { get; }
        bool IsSystem { get; }
        string Hash { get; }
        bool IsBlocked { get; }
    }
}