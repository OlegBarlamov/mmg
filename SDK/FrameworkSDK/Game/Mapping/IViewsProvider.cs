using System;
using System.Collections.Generic;

namespace FrameworkSDK.Game.Mapping
{
	public interface IViewsProvider
	{
		IEnumerable<Type> GetRegisteredViews();
	}
}
