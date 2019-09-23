using System;
using System.Collections.Generic;

namespace FrameworkSDK.Game.Mapping
{
	public interface IControllersProvider
	{
		IEnumerable<Type> GetRegisteredControllers();
	}
}
