namespace FrameworkSDK.Configuration
{
    public interface IConfigurationPhaseAction : INamed
    {
        bool IsCritical { get; }

        void Process(NamedObjectsHeap configureContext);
    }
}