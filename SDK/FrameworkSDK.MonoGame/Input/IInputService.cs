namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IInputService
    {
        IKeyboardProvider Keyboard { get; }
        
        IMouseProvider Mouse { get; }
        
        IGamepadProvider Gamepads { get; }
    }
}