using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IClosable : IUpdatable<ClosingState>
	{
		void OnClosed();
	}
}
