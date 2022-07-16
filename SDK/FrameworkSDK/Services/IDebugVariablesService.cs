namespace FrameworkSDK.Services
{
    public interface IDebugVariablesService
    {
        T GetValue<T>(string key);

        void SetValue<T>(string key, T value);
    }
}