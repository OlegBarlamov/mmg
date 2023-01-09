using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BoardPlatform.Data
{
    public class Widget : IWidget
    {
        public IPosition2D GetPosition()
        {
            return Position;
        }

        public ISize2D GetSize()
        {
            return Size;
        }

        public IToken GetParentBoardId()
        {
            return ParentBoardToken;
        }

        public IToken GetToken()
        {
            return Token;
        }

        public IRawWidgetData GetRawData()
        {
            return DictionaryBasedRawWidgetData.FromDictionary(new Dictionary<string, object>
            {
                {"position", Position},
                {"size", Size},
            });
        }
        
        private IToken Token { get; }
        private IToken ParentBoardToken { get; }
        public IPosition2D Position { get; }
        public ISize2D Size { get; }

        private Widget([NotNull] IToken token, [NotNull] IToken parentBoardToken, [NotNull] IPosition2D position,
            [NotNull] ISize2D size)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ParentBoardToken = parentBoardToken ?? throw new ArgumentNullException(nameof(parentBoardToken));
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Size = size ?? throw new ArgumentNullException(nameof(size));
        }

        public static IWidget FromRawWidgetData([NotNull] IToken token, [NotNull] IToken parentBoardToken, IRawWidgetData rawWidgetData)
        {
            return new Widget(token, parentBoardToken, rawWidgetData.GetValue<IPosition2D>("position"), rawWidgetData.GetValue<ISize2D>("size"));
        }
    }
}