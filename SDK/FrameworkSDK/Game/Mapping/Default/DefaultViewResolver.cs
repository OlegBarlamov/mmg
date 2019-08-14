using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkSDK.Game.Views;

namespace FrameworkSDK.Game.Mapping.Default
{
    internal class DefaultViewResolver : IViewResolver
    {
        public IView ResolveByModel(object model)
        {
            throw new NotImplementedException();
        }

        public bool IsModelHasView(object model)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
