using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    public class RegisteredServicesMapping
    {
        private readonly Dictionary<int, List<IRegistrationInfo>> _mapping = new Dictionary<int, List<IRegistrationInfo>>();
        
        public RegisteredServicesMapping(IReadOnlyCollection<IRegistrationInfo> registrations)
        {
            foreach (var registrationInfo in registrations)
            {
                var code = GetCode(registrationInfo.Type);
                if (!_mapping.ContainsKey(code))
                    _mapping.Add(code, new List<IRegistrationInfo>());

                _mapping[code].Add(registrationInfo);
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _mapping.Values.SelectMany(list => list.Select(regInfo => regInfo.ToString())));
        }

        public IReadOnlyList<IRegistrationInfo> FindByType(Type type)
        {
            var code = GetCode(type);
            if (_mapping.TryGetValue(code, out var registrations))
                return registrations;
            return Array.Empty<IRegistrationInfo>();
        }
        
        private static int GetCode([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.GetHashCode();
        }
    }
}