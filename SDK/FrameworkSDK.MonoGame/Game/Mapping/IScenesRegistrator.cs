using System;

namespace FrameworkSDK.Game.Mapping
{
    public interface IScenesRegistrator
    {
        void RegisterScene(Type modelType, Type sceneType);
    }
}
