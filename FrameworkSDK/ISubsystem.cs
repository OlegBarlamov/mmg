using System;

namespace FrameworkSDK
{
	public interface ISubsystem : IDisposable
	{
		void Initialize();

		void OnGameActivated(Application application);

		void OnGameDeactivated(Application application);

		void OnGameEnding(Application application);
	}
}
