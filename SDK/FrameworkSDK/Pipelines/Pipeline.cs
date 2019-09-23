using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Pipeline
    {
		[NotNull]
	    public static Pipeline Empty => new Pipeline();

        [NotNull, ItemNotNull]
        public List<PipelineStep> Steps { get; } = new List<PipelineStep>();

        [NotNull]
        public PipelineStep this[string stepName]
        {
            get
            {
                if (stepName == null) throw new ArgumentNullException(nameof(stepName));
                if (string.IsNullOrWhiteSpace(stepName)) throw new ArgumentException(nameof(stepName));

                if (!Steps.ContainsByName(stepName))
                    throw new PipelineException(Strings.Exceptions.Pipeline.StepNotFound, stepName);

                return Steps.First(phase => phase.Name == stepName);
            }
        }

        public virtual IReadOnlyList<PipelineStep> GetStepsForProcess()
        {
            return Steps.ToArray();
        }
    }
}