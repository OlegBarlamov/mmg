using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
    public class Pipeline
    {
        [NotNull, ItemNotNull]
        public List<PipelineStep> Steps { get; } = new List<PipelineStep>();

        [NotNull]
        public PipelineStep this[string phaseName]
        {
            get
            {
                if (phaseName == null) throw new ArgumentNullException(nameof(phaseName));
                if (string.IsNullOrWhiteSpace(phaseName)) throw new ArgumentException(nameof(phaseName));

                if (!Steps.ContainsWithName(phaseName))
                    throw new PipelineException();

                return Steps.First(phase => phase.Name == phaseName);
            }
        }

        public virtual IReadOnlyList<PipelineStep> GetStepsForProcess()
        {
            return Steps.ToArray();
        }
    }
}