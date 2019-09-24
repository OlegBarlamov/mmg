using NetExtensions;

namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IGameParameters
    {
        string ContentRootDirectory { get; }
        Int32Size BackBufferSize { get; }
        bool IsFullScreenMode { get; }
    }
}
