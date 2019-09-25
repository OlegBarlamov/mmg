using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IModelsProvider
	{
		IEnumerable<Type> GetRegisteredModels();
	}
}
