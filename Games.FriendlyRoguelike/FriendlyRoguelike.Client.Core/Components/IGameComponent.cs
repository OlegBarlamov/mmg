using FriendlyRoguelike.Core.Models;

namespace FriendlyRoguelike.Core.Components
{
    public interface IGameComponent
    {
        void Update(GameTimeTicks gameTime);
    }
}