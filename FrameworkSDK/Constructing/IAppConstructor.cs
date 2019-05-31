using FrameworkSDK.IoC;

namespace FrameworkSDK.Constructing
{
    public interface IAppConstructor
    {
        IServiceRegistrator ServiceRegistrator { get; }

        void RegisterSubsystem(ISubsystem subsystem);
    }
}
