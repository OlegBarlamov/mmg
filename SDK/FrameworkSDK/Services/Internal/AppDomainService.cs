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
