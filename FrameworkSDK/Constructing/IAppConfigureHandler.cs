using System;
using FrameworkSDK.Pipelines;

namespace FrameworkSDK.Constructing
{
    public interface IAppConfigureHandler : IDisposable
    {
        Pipeline ConfigurationPipeline { get; }

        /// <summary>
        /// Осуществляет построение приложения.
        /// </summary>
        void Configure();
    }
}