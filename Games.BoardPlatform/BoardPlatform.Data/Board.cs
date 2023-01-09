using System;
using BoardPlatform.Data.Repositories;
using JetBrains.Annotations;

namespace BoardPlatform.Data
{
    public class Board : IBoard
    {
        public IToken Token { get; }

        public Board([NotNull] IToken token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
        
        public IToken GetToken()
        {
            return Token;
        }

        public IOnBoardWidgetProvider GetWidgetsProvider()
        {
            throw new NotImplementedException();
        }
    }
}