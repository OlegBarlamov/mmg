using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK
{
    public sealed class AppFactory
    {
		[NotNull] public IPipelineProcessor PipelineProcessor { get; set; }

	    public AppFactory([CanBeNull] IFrameworkLogger factoryLogger)
	    {
		    PipelineProcessor = new AppPipelineProcessor(factoryLogger);
	    }

	    public AppFactory()
			:this(null)
	    {
	    }

	    public IAppConfigurator Create()
	    {
			return new AppConfigurator(PipelineProcessor)
			{
				ConfigurationPipeline = new DefaultConfigurationFactory().Create()
			};
		}

	    public IAppConfigurator Create(Pipeline configurationPipeline)
	    {
		    return new AppConfigurator(PipelineProcessor)
		    {
			    ConfigurationPipeline = configurationPipeline
			};
	    }

		public IAppConfigurator CreateEmpty()
	    {
		    return new AppConfigurator(PipelineProcessor)
		    {
			    ConfigurationPipeline = Pipeline.Empty
		    };
	    }
    }
}
