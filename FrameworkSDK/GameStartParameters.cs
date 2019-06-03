using NetExtensions;

namespace FrameworkSDK
{
	public class GameStartParameters
	{
		public string ContentRootDirectory { get; set; } = "Content";

        public Int32Size BackBufferSize { get; set; } = new Int32Size(1024, 768);

        public bool IsFullScreenMode { get; set; }
    }
}
