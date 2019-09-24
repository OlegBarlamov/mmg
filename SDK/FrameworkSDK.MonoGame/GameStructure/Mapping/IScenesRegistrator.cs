using System;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public interface IScenesRegistrator
    {
        void RegisterScene(Type modelType, Type sceneType);
    }
}
