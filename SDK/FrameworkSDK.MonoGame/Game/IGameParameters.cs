using NetExtensions;

namespace FrameworkSDK.Game
{
    public interface IGameParameters
    {
        string ContentRootDirectory { get; }
        Int32Size BackBufferSize { get; }
        bool IsFullScreenMode { get; }
    }
}
