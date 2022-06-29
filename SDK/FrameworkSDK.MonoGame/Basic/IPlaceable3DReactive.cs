using System;

namespace FrameworkSDK.MonoGame.Basic
{
    public interface IPlaceable3DReactive : IPlaceable3D
    {
        event EventHandler PlacementChanged;
    }
}