namespace FrameworkSDK.MonoGame.InputManagement
{
    public class GamePadEventHandlerArgs
    {
        public int PlayerIndex { get; }
        public IGamePadDataSource DataSource { get; }

        internal GamePadEventHandlerArgs(int playerIndex, IGamePadDataSource dataSource)
        {
            PlayerIndex = playerIndex;
            DataSource = dataSource;
        }
    }
}