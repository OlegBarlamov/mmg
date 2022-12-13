namespace BoardPlatform.Data.Tokens
{
    public interface ITokensFactory
    {
        IToken CreateBoardToken();
        
        IToken CreateWidgetToken();
    }
}