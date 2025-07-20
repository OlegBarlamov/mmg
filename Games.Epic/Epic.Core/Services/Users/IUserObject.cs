using System;
using Epic.Core.Objects;
using Epic.Data;

namespace Epic.Core.Services.Users
{
    public interface IUserObject : IGameObject<IUserEntity>
    {
        Guid Id { get; }
        string Name { get; }
        bool IsSystem { get; }
        string Hash { get; }
        bool IsBlocked { get; }
    }
}