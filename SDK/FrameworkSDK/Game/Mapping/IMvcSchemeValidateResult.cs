namespace FrameworkSDK.Game.Mapping
{
    public interface IMvcSchemeValidateResult
    {
        bool IsModelExist { get; }
        bool IsViewExist { get; }
        bool IsControllerExist { get; }
    }
}
