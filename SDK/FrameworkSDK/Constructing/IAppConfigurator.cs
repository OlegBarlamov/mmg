namespace FrameworkSDK.Constructing
{
    public interface IAppConfigurator : IAppConfigureHandler
    {
        //Конфигурирует и запускает приложение.
        void Run();
    }
}