using System;
using FrameworkSDK.Game.Controllers;

namespace FrameworkSDK.Game.Mapping.Default
{
    internal class DefaultControllerResolver : IControllerResolver
    {
        public IController ResolveByModel(object model)
        {
            throw new NotImplementedException();
        }

        public bool IsModelHasController(object model)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
