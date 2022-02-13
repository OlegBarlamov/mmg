using System;
using System.Collections.Generic;
using FrameworkSDK.Services;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class DefaultAppContainer : IApp
    {
        private IReadOnlyCollection<IAppSubSystem> SubSystems { get; }

        private IAppSubSystemsRunner AppSubSystemsRunner { get; }
        
        public DefaultAppContainer([NotNull] IReadOnlyCollection<IAppSubSystem> subSystems, [NotNull] IAppSubSystemsRunner appSubSystemsRunner)
        {
            AppSubSystemsRunner = appSubSystemsRunner ?? throw new ArgumentNullException(nameof(appSubSystemsRunner));
            SubSystems = subSystems ?? throw new ArgumentNullException(nameof(subSystems));
        }

        public void Run()
        {
            AppSubSystemsRunner.Run(SubSystems);
        }

        public void Dispose()
        {
            foreach (var subSystem in SubSystems)
            {
                subSystem.Dispose();
            }
        }
    }
}