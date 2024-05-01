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

        public SphereObjectData AddSphereObject(Color color, Vector2 position, float size, Teams team)
        {
            var data = new SphereObjectData(color, position, size, team);

            Scene.AddView(data);

            return data;
        }

        public SphereObjectData CreateBulletWorkpiece(PlayerData player, Vector2 origin, float size = 3f)
        {
            var bulletPosition = player.Position + origin * (player.Size + size);
            if (player.Velocity != Vector2.Zero && Vector2.Dot(Vector2.Normalize(player.Velocity), origin) > 0)
                bulletPosition += player.Velocity;
            
            var data = AddSphereObject(player.Color, bulletPosition, size, player.Team);
            data.NoClipMode = true;
            
            TakeDamage(player, size);

            return data;
        }

        public void ReleaseBullet(PlayerData player, SphereObjectData bullet, Vector2 bulletVelocity)
        {
            bullet.NoClipMode = false;
            var impulse = bulletVelocity * bullet.Parameters.Mass;
            Scene.Physics2D.ApplyImpulse(bullet, impulse);
            Scene.Physics2D.ApplyImpulse(player, -impulse);
        }

        public void HandleConsumption(SphereObjectData sphereA, SphereObjectData sphereB, GameTime gameTime)
        {
            if (sphereA.Team.IsAllyWith(sphereB.Team))
                return;
            
            var biggerSphere = sphereA.Size > sphereB.Size ? sphereA : sphereB;
            var smallerSphere = sphereA.Size > sphereB.Size ? sphereB : sphereA;
            var consumptionSpeed = 1 * gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            if (sphereA.Team.IsSelf(sphereB.Team))
            {
                IncreaseSize(biggerSphere, consumptionSpeed);
                TakeDamage(smallerSphere, consumptionSpeed);
            }
            if (sphereA.Team.IsNeutralWith(sphereB.Team))
            {
                IncreaseSize(biggerSphere, consumptionSpeed / 2);
                TakeDamage(smallerSphere, consumptionSpeed);
                var normal = Vector2.Normalize(biggerSphere.Position - smallerSphere.Position);
                smallerSphere.SetPosition(smallerSphere.Position + normal * consumptionSpeed);
            }
            if (sphereA.Team.IsEnemyWith(sphereB.Team))
            {
                consumptionSpeed = consumptionSpeed * 4;
                TakeDamage(biggerSphere, consumptionSpeed);
                TakeDamage(smallerSphere, consumptionSpeed);
                var normal = Vector2.Normalize(biggerSphere.Position - smallerSphere.Position);
                // r1 + r2 = r1 - consumptionSpeed + r2 - consumptionSpeed +  
                smallerSphere.SetPosition(smallerSphere.Position + normal * consumptionSpeed * 4);
            }
            
            
        }

        public void TakeDamage(SphereObjectData sphere, float damage)
        {
            sphere.SetSize(Math.Max(0, sphere.Size - damage));
            if (sphere.Size <= 0)
            {
                sphere.Dead = true;
                Scene.RemoveView(sphere);
            }
        }
        
        public void IncreaseSize(SphereObjectData sphere, float size)
        {
            sphere.SetSize(sphere.Size + size);
        }
    }
}