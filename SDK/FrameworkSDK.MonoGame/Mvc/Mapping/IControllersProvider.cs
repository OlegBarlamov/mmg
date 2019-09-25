using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IControllersProvider
	{
		IEnumerable<Type> GetRegisteredControllers();
	}
}
