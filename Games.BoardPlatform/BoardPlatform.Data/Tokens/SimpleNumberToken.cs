using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;

namespace BoardPlatform.Data.Tokens
{
    public class SimpleNumberToken : IToken
    {
        private string Id { get; }

        public static IToken Generate()
        {
            var hash = Hash.Generate(HashType.BigNumber);
            return new SimpleNumberToken(hash.Value);
        }

        public static IToken FakeToken(string token)
        {
            return new SimpleNumberToken(token);
        }

        internal SimpleNumberToken([NotNull] string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
        
        public string GetId()
        {
            return Id;
        }

        public bool Equals(IToken other)
        {
            return Id == other?.GetId();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleNumberToken) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(SimpleNumberToken left, SimpleNumberToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimpleNumberToken left, SimpleNumberToken right)
        {
            return !Equals(left, right);
        }
    }
}