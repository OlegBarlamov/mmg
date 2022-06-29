using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Graphics.Services;

namespace FrameworkSDK.MonoGame.Services
{
    public class HighPolygonalServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IIndicesBuffersFactory, Int32IndicesBuffersFactory>();
            serviceRegistrator.RegisterType<IIndicesBuffersFiller, Int32IndicesBuffersFiller>();
        }
    }
}