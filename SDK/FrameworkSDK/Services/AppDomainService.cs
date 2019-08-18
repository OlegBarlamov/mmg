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

	    private List<Type> _bufferedTypes;
	    private bool _isDisposed;

        [NotNull] public IEnumerable<Type> GetAllTypes()
        {
			if (_isDisposed) throw new ObjectDisposedException(nameof(AppDomainService));

	        lock (_locker)
	        {
		        if (_bufferedTypes != null)
			        return _bufferedTypes;

		        _bufferedTypes = AppDomain.CurrentDomain.GetAllTypes().ToList();
		        return _bufferedTypes;
	        }
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
