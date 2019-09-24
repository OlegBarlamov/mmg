using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.GameStructure
{
	public interface IModelsProvider
	{
		IEnumerable<Type> GetRegisteredModels();
	}
}
