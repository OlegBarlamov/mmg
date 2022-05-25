using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface ISceneComponent
	{
		[CanBeNull] SceneBase OwnedScene { get; }

	    void OnAddedToScene([NotNull] SceneBase scene);

	    void OnRemovedFromScene([NotNull] SceneBase scene);
	}
}
