using System;
using System.Reflection;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
	internal interface IConstructorFinder
	{
		[NotNull] ConstructorInfo FindConstructor([NotNull] Type type);
	}
}
