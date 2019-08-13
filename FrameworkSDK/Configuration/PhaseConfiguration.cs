using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
    public class PhaseConfiguration
    {
        [NotNull, ItemNotNull]
        public List<ConfigurationPhase> Phases { get; } = new List<ConfigurationPhase>();

        [NotNull]
        public ConfigurationPhase this[string phaseName]
        {
            get
            {
                if (phaseName == null) throw new ArgumentNullException(nameof(phaseName));
                if (string.IsNullOrWhiteSpace(phaseName)) throw new ArgumentException(nameof(phaseName));

                if (!Phases.ContainsWithName(phaseName))
                    throw new ConfigurationException();

                return Phases.First(phase => phase.Name == phaseName);
            }
        }

        public virtual IReadOnlyList<ConfigurationPhase> GetPhasesForProcess()
        {
            return Phases.ToArray();
        }
    }
}