
namespace FrameworkSDK.MonoGame.Mvc
{
	public sealed class EmptyScene : SceneBase
	{
		public override bool ReadyToBeClosed
		{
			get => true;
			protected set { }
		}

		protected override bool IsInitialized { get; } = true;

		public EmptyScene()
	        :base("empty_scene")
	    {
		    
	    }
	}
}
