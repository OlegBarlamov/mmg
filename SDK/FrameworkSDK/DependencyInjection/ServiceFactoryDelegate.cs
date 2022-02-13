using System;

namespace FrameworkSDK.DependencyInjection
{
    public delegate object ServiceFactoryDelegate(IServiceLocator serviceLocator, Type requestedType);
}