using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.DependencyInjection.Default.Models;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public class DefaultServicesRegistrationsPriority : IServicesRegistrationsPriority
    {
        public IRegistrationInfo GetPreferable(Type targetType, IReadOnlyList<IRegistrationInfo> registrations)
        {
            // Simple registrations 
            var simpleRegistrationCandidate = registrations.LastOrDefault(registration =>
                registration is TypeRegistrationInfo || registration is InstanceRegistrationInfo);
            if (simpleRegistrationCandidate != null)
                return simpleRegistrationCandidate;
            
            // Other
            return registrations.Last();
        }
    }
}