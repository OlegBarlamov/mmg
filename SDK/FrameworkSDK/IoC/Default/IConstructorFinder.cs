using System;
using System.Reflection;

namespace FrameworkSDK.IoC.Default
{
    public delegate bool ParameterResolvableDelegate(Type parameterType);

    public interface IConstructorFinder
    {
        ConstructorInfo FindConstructor(ParameterResolvableDelegate parameterResolvableFunc);
    }
}
