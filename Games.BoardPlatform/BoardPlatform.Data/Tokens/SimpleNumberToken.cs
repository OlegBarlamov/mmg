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

        private SimpleNumberToken([NotNull] string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
        
        public string GetId()
        {
            return Id;
        }
    }
}