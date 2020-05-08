using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IInputManager : IUpdateable
    {
        IInputService InputService { get; }
    }
}