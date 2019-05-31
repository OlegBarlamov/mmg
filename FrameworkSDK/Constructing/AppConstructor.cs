using System;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConstructor : IAppConstructor, IDisposable
    {
        public IServiceRegistrator ServiceRegistrator { get; }

        private IApplication Application { get; }

        private bool _isDisposed;

        public AppConstructor([NotNull] IApplication application, [NotNull] IServiceRegistrator serviceRegistrator)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            ServiceRegistrator = serviceRegistrator ?? throw new ArgumentNullException(nameof(serviceRegistrator));
        }

        public void RegisterSubsystem([NotNull] ISubsystem subsystem)
        {
            if (subsystem == null) throw new ArgumentNullException(nameof(subsystem));

            CheckDisposed();

            Application.RegisterSubsystem(subsystem);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (_isDisposed) throw new ConstructionStateFinishedException();
        }
    }

    public class ConstructionStateFinishedException : FrameworkException
    {
        public ConstructionStateFinishedException()
            : base(Strings.Exceptions.ConstructionStateFinished)
        {
        }
        
    }

}