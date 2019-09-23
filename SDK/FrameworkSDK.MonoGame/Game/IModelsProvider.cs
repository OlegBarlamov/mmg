using System;
using System.Collections.Generic;

namespace FrameworkSDK.Game
{
	public interface IModelsProvider
	{
		IEnumerable<Type> GetRegisteredModels();
	}
}
