using System.Reflection;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
	internal interface IDependencyResolver
	{
		[NotNull] object[] ResolveDependencies([NotNull] ConstructorInfo constructor);
	}
}
