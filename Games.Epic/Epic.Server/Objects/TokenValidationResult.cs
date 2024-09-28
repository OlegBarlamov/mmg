using JetBrains.Annotations;

namespace Epic.Server.Objects
{
    public interface ITokenValidationResult
    {
        bool IsValid { get; }
        ISessionObject Session { get; }
        InvalidTokenReason Reason { get; }
    }

    public enum InvalidTokenReason
    {
        None,
        Invalid, 
        Revoked,
    }
    
    class TokenInvalidValidationResult : ITokenValidationResult
    {
        public bool IsValid { get; } = false;
        public ISessionObject Session { get; } = null;
        public InvalidTokenReason Reason { get; } = InvalidTokenReason.Invalid;
    }

    class TokenRevokedValidationResult  : ITokenValidationResult
    {
        public bool IsValid { get; } = false;
        public ISessionObject Session { get; }
        public InvalidTokenReason Reason { get; } = InvalidTokenReason.Revoked;

        public TokenRevokedValidationResult(ISessionObject session)
        {
            Session = session;
        }
    }

    class TokenValidValidationResult : ITokenValidationResult
    {
        public bool IsValid { get; } = true;
        public ISessionObject Session { get; }
        public InvalidTokenReason Reason { get; } = InvalidTokenReason.None;

        public TokenValidValidationResult(ISessionObject session)
        {
            Session = session;
        }
    }
}