using System;

namespace TablePlatform.Data.Tokens
{
    public class SimpleGuidToken : IToken
    {
        public Guid Id { get; }

        public SimpleGuidToken(Guid id)
        {
            Id = id;
        }
        
        public string GetId()
        {
            return Id.ToString();
        }
    }
}