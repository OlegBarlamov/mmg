namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IMvcSchemeValidateResult
    {
        bool IsModelExist { get; }
        bool IsViewExist { get; }
        bool IsControllerExist { get; }
    }
}
