using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    [UsedImplicitly]
    public class MvcAppComponent : IAppComponent
    {
        private IMvcMappingProvider MvcMappingProvider { get; }

        public MvcAppComponent([NotNull] IMvcMappingProvider mvcMappingProvider)
        {
            MvcMappingProvider = mvcMappingProvider ?? throw new ArgumentNullException(nameof(mvcMappingProvider));
        }
        
        public void Dispose()
        {
            
        }

        public void Configure()
        {
            MvcMappingProvider.FetchMapping();
        }
    }
}