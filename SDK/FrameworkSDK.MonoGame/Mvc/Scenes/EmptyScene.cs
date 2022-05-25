
namespace FrameworkSDK.MonoGame.Mvc
{
	public sealed class EmptyScene : SceneBase
	{
		public override bool ReadyToBeClosed
		{
			get => true;
			protected set { }
		}

		public EmptyScene()
	        :base("empty_scene")
	    {
		    
	    }
	}
}
