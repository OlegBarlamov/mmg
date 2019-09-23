using System;
using FrameworkSDK.Pipelines;

namespace FrameworkSDK.Constructing
{
    public interface IAppConfigurator : IDisposable
    {
        Pipeline ConfigurationPipeline { get; }

        IAppRunner Configure();
    }
}