using JetBrains.Annotations;

namespace FrameworkSDK.Game.Scenes
{
	public interface ISceneComponent
	{
		[CanBeNull] Scene OwnedScene { get; }

	    void OnAddedToScene([NotNull] Scene scene);

	    void OnRemovedFromScene([NotNull] Scene scene);
	}
}
