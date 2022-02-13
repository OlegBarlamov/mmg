using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection.Default.Models;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public interface IServicesRegistrationsPriority
    {
        IRegistrationInfo GetPreferable(Type targetType, IReadOnlyList<IRegistrationInfo> registrations);
    }
}