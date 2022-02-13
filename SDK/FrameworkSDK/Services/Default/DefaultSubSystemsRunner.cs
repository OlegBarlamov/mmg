using System;
using System.Collections.Generic;
using FrameworkSDK.Constructing;
using JetBrains.Annotations;

namespace FrameworkSDK.Services.Default
{
    [UsedImplicitly]
    internal class DefaultSubSystemsRunner : IAppSubSystemsRunner
    {
        public void Run([NotNull] IReadOnlyCollection<IAppSubSystem> subSystems)
        {
            if (subSystems == null) throw new ArgumentNullException(nameof(subSystems));
            foreach (var subSystem in subSystems)
            {
                subSystem.Run();
            }
        }
    }
}