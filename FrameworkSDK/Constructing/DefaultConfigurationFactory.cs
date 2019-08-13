using FrameworkSDK.Configuration;
using FrameworkSDK.Localization;

namespace FrameworkSDK.Constructing
{
    internal class DefaultConfigurationFactory
    {
        public PhaseConfiguration Create()
        {
            var configuration = new PhaseConfiguration();

            var initializePhase = new ConfigurationPhase(DefaultConfigurationSteps.Initialization);

            initializePhase.AddAction(new SimpleConfigurationAction(
                DefaultConfigurationSteps.InitializationActions.Localization, true,
                heap =>
                {
                    var localization = new DefaultLocalization();
                    heap.SetObject("localization", localization);
                }));

            return configuration;
        }
    }


}
