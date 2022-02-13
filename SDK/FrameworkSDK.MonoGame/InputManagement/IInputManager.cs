using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IInputManager : IUpdatable
    {
        IInputService InputService { get; }
    }
}