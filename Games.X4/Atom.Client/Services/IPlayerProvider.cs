using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;

namespace Atom.Client.Services
{
    public interface IPlayerProvider : IUpdatable
    {
        Vector3 GetPlayerPosition();
    }
}