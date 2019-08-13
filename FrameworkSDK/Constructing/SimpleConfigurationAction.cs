using System;
using FrameworkSDK.Configuration;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class SimpleConfigurationAction : IConfigurationPhaseAction
    {
        public string Name { get; }
        public bool IsCritical { get; }

        private Action<NamedObjectsHeap> ProcessAction { get; }

        public SimpleConfigurationAction([NotNull] string name, bool isCritical, [NotNull] Action<NamedObjectsHeap> processAction)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsCritical = isCritical;
            ProcessAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
        }

        public SimpleConfigurationAction([NotNull] string name, [NotNull] Action<NamedObjectsHeap> processAction)
            : this(name, false, processAction)
        {

        }

        public void Process(NamedObjectsHeap configureContext)
        {
            ProcessAction.Invoke(configureContext);
        }
    }
}
