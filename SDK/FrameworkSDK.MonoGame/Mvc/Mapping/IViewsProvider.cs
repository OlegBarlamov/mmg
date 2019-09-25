using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IViewsProvider
	{
		IEnumerable<Type> GetRegisteredViews();
	}
}
