using System.Collections.Generic;
using FrameworkSDK.Constructing;

namespace FrameworkSDK.Services
{
    public interface IAppSubSystemsRunner
    {
        void Run(IReadOnlyCollection<IAppSubSystem> subSystems);
    }
}