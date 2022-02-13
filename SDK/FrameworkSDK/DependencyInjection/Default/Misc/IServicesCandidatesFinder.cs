using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection.Default.Models;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public interface IServicesCandidatesFinder
    {
        IReadOnlyList<IRegistrationInfo> FindCandidates(Type type, RegisteredServicesMapping mapping);
    }
}