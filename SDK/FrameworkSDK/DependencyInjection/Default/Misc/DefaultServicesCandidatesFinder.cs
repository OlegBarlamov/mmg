using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection.Default.Models;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public class DefaultServicesCandidatesFinder : IServicesCandidatesFinder
    {
        public IReadOnlyList<IRegistrationInfo> FindCandidates(Type type, [NotNull] RegisteredServicesMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            var result = new List<IRegistrationInfo>(mapping.FindByType(type));
            if (type.IsGenericType)
                result.AddRange(mapping.FindByType(type.GetGenericTypeDefinition()));

            return result;
        }
    }
}