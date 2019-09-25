using System;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IScenesRegistrator
    {
        void RegisterScene(Type modelType, Type sceneType);
    }
}
