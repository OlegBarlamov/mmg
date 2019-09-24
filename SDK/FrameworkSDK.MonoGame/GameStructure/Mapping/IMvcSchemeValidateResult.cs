namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public interface IMvcSchemeValidateResult
    {
        bool IsModelExist { get; }
        bool IsViewExist { get; }
        bool IsControllerExist { get; }
    }
}
