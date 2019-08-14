namespace FrameworkSDK.Pipelines
{
    public interface IPipelineAction : INamed
    {
        bool IsCritical { get; }

        void Process(IPipelineContext pipelineContext);
    }
}