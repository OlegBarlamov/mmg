using FrameworkSDK.Game;
using FrameworkSDK.Game.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal interface IApplication : IUpdatable
    {
	    [CanBeNull] Scene CurrentScene { get; }

		void RegisterSubsystem([NotNull] ISubsystem subsystem);
    }
}
