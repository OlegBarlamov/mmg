using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IMvcMappingProvider
    {
        void FetchMapping();
        IReadOnlyList<MvcTypesDeclaration> GetMapping();
    }
}