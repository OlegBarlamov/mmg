using System;

namespace BoardPlatform.Data
{
    public interface IToken : IEquatable<IToken>
    {
        string GetId();
    }
}