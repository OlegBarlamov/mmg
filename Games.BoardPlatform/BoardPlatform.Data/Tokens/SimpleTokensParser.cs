using System;
using JetBrains.Annotations;

namespace BoardPlatform.Data.Tokens
{
    public class SimpleTokensParser : ITokensParser
    {
        public IToken FromBoardId([NotNull] string queryToken)
        {
            if (string.IsNullOrWhiteSpace(queryToken)) throw new ArgumentException(nameof(queryToken));

            return new SimpleNumberToken(queryToken);
        }

        public IToken FromWidgetId(string widgetId)
        {
            return new SimpleNumberToken(widgetId);
        }
    }
}