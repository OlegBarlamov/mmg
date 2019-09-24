using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
	public interface IControllersProvider
	{
		IEnumerable<Type> GetRegisteredControllers();
	}
}
