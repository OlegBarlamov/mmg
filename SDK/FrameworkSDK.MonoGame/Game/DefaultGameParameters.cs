using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game
{
    [UsedImplicitly]
    internal class DefaultGameParameters : IGameParameters
    {
        public string ContentRootDirectory { get; } = "Content";
        public Int32Size BackBufferSize { get; } = new Int32Size(640, 480);
        public bool IsFullScreenMode { get; } = false;
    }
}
