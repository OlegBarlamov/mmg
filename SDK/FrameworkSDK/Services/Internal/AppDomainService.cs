using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Services
{
    [UsedImplicitly]
    internal class AppDomainService : IAppDomainService, IDisposable
    {
		private readonly object _locker = new object();

	    private volatile List<Type> _bufferedTypes;
	    private bool _isDisposed;

        public IEnumerable<Type> GetAllTypes()
        {
			if (_isDisposed) throw new ObjectDisposedException(nameof(AppDomainService));

			if (_bufferedTypes != null)
				return _bufferedTypes;
			
	        lock (_locker)
	        {
		        if (_bufferedTypes != null)
			        return _bufferedTypes;
		        
		        var bufferedTypes = AppDomain.CurrentDomain.GetAllTypes().ToList();
		        _bufferedTypes = bufferedTypes;
	        }
	        
	        return _bufferedTypes;
        }

        public Type FindTypeFromFullName(string typeName)
        {
	        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
	        {
		        var type = assembly.GetType(typeName);
		        if (type == null)
			        continue;
		        return type;
	        }
	        return null;
        }

        public Type FindTypeFromShortName(string name)
        {
	        return GetAllTypes().FirstOrDefault(type =>
		        string.Equals(name, type.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Dispose()
	    {
		    lock (_locker)
		    {
			    _isDisposed = true;
				_bufferedTypes.Clear();
		    }
	    }
    }
}
