using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Models.SphereObject;

namespace Omegas.Client.MacOs.Services
{
    public class OmegaGameService
    {
        private MainScene Scene { get; }

        public OmegaGameService([NotNull] MainScene scene)
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
        }

        public PlayerData AddPlayer(PlayerIndex playerIndex, Vector2 position)
        {
            var data = new PlayerData(playerIndex, Color.Red, position, 50);

            Scene.AddView(data);

            return data;
        }

        public SphereObjectData AddSphereObject(Color color, Vector2 position, float size)
        {
            var data = new SphereObjectData(color, position, size);

            Scene.AddView(data);

            return data;
        }

        public SphereObjectData CreateBulletWorkpiece(PlayerData player, Vector2 origin, Color color, float size = 5f)
        {
            var bulletPosition = player.Position + origin * (player.Size + size);
            if (player.Velocity != Vector2.Zero && Vector2.Dot(Vector2.Normalize(player.Velocity), origin) > 0)
                bulletPosition += player.Velocity;

            var data = AddSphereObject(color, bulletPosition, size);
            data.NoClipMode = true;

            return data;
        }

        public void ReleaseBullet(PlayerData player, SphereObjectData bullet, Vector2 bulletVelocity)
        {
            bullet.NoClipMode = false;
            var impulse = bulletVelocity * bullet.Parameters.Mass;
            Scene.Physics2D.ApplyImpulse(bullet, impulse);
            Scene.Physics2D.ApplyImpulse(player, -impulse);
        }
    }
}