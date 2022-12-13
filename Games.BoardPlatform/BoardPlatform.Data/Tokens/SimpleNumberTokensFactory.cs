namespace BoardPlatform.Data.Tokens
{
    public class SimpleNumberTokensFactory : ITokensFactory
    {
        public IToken CreateBoardToken()
        {
            return SimpleNumberToken.Generate();
        }

        public IToken CreateWidgetToken()
        {
            return SimpleNumberToken.Generate();
        }
    }
}