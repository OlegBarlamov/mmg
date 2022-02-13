using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    internal class RegistrationsDomain
    {
        private readonly List<IRegistrationInfo> _allRegistrations;
        private readonly List<IRegistrationInfo> _newRegistrations = new List<IRegistrationInfo>();

        public RegistrationsDomain()
            : this(Array.Empty<IRegistrationInfo>())
        {
        }

        public RegistrationsDomain([NotNull] IEnumerable<IRegistrationInfo> initialRegistrations)
        {
            if (initialRegistrations == null) throw new ArgumentNullException(nameof(initialRegistrations));
            _allRegistrations = new List<IRegistrationInfo>(initialRegistrations);
        }

        public void Add([NotNull] IRegistrationInfo registrationInfo)
        {
            if (registrationInfo == null) throw new ArgumentNullException(nameof(registrationInfo));
            _newRegistrations.Add(registrationInfo);
            _allRegistrations.Add(registrationInfo);
        }

        public IReadOnlyCollection<IRegistrationInfo> GetAll()
        {
            return _allRegistrations;
        }
        
        public IReadOnlyCollection<IRegistrationInfo> GetCurrent()
        {
            return _newRegistrations;
        }

        public RegistrationsDomain CreateScoped()
        {
            return new RegistrationsDomain(_allRegistrations);
        }
    }
}