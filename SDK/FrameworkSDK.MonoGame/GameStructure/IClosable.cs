using FrameworkSDK.MonoGame.GameStructure;

namespace FrameworkSDK.MonoGame.GameStructure
{
	public interface IClosable : IUpdatable<ClosingState>
	{
		void OnClosed();
	}
}
