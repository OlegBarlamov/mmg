using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IClosable : IUpdatable
	{
		bool ReadyToBeClosed { get; }
		void CloseRequest();
		
		void OnClosed();
	}
}
