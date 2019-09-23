namespace FrameworkSDK.Game
{
	public interface IClosable : IUpdatable<ClosingState>
	{
		void OnClosed();
	}
}
