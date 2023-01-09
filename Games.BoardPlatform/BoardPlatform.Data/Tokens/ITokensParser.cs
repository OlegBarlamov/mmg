namespace BoardPlatform.Data.Tokens
{
    public interface ITokensParser
    {
        IToken FromBoardId(string queryString);
        IToken FromWidgetId(string widgetId);
    }
}