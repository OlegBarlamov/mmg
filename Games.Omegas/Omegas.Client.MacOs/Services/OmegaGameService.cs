using System;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Models.SphereObject;

namespace Omegas.Client.MacOs.Services
{
    public class OmegaGameService
    {
        private MainScene Scene { get; }
        public IDebugInfoService DebugInfoService { get; }

        public OmegaGameService([NotNull] MainScene scene, [NotNull] IDebugInfoService debugInfoService)
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
        }

        public SphereObjectData AddSphereObject(Color color, Vector2 position, float size, Teams team)
        {
            var data = new SphereObjectData(color, position, size, team);

            Scene.AddView(data);

            return data;
        }

        public SphereObjectData CreateBulletWorkpiece(PlayerData player, Vector2 originNormal)
        {
            var bulletSize = Math.Max(2, player.Size / 50);
            var bulletPosition = player.Position + originNormal * (player.Size + bulletSize);
            
            var data = AddSphereObject(player.Color, bulletPosition, bulletSize, player.Team);
            data.NoClipMode = true;
            
            TakeDamage(player, bulletSize);

            return data;
        }

        public void CancelBullet(PlayerData player, SphereObjectData bullet)
        {
            IncreaseSize(player, bullet.Size);
            Kill(bullet);
        }

        public void ReleaseBullet(PlayerData player, SphereObjectData bullet, Vector2 origin, Vector2 originNormal)
        {
            var targetVelocity = player.Velocity + origin * 20f;
            if (player.Velocity != Vector2.Zero && Vector2.Dot(Vector2.Normalize(player.Velocity), originNormal) > 0)
                bullet.SetPosition(bullet.Position + player.Velocity);
            
            bullet.NoClipMode = false;
            var impulse = targetVelocity * bullet.Parameters.Mass;
            Scene.Physics2D.ApplyImpulse(bullet, impulse);
            Scene.Physics2D.ApplyImpulse(player, -impulse);
        }

        public void FillBullet(PlayerData playerData, SphereObjectData bullet, float factor, GameTime gameTime)
        {
            var fillingSpeed = factor * gameTime.ElapsedGameTime.Milliseconds * 0.005f;
            TakeDamage(playerData, fillingSpeed);
            IncreaseSize(bullet, fillingSpeed);
        }

        public void HandleConsumption(SphereObjectData sphereA, SphereObjectData sphereB, GameTime gameTime)
        {
            if (sphereA.Team.IsAllyWith(sphereB.Team))
                return;
            
            var biggerSphere = sphereA.Size > sphereB.Size ? sphereA : sphereB;
            var smallerSphere = sphereA.Size > sphereB.Size ? sphereB : sphereA;
            var consumptionSpeed = 1 * gameTime.ElapsedGameTime.Milliseconds * 0.01f;

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
                TakeDamage(biggerSphere, consumptionSpeed);
                TakeDamage(smallerSphere, consumptionSpeed);
                var normal = Vector2.Normalize(biggerSphere.Position - smallerSphere.Position);
                smallerSphere.SetPosition(smallerSphere.Position + normal * consumptionSpeed * 3f);
            }
        }

        public void TakeDamage(SphereObjectData sphere, float damage)
        {
            sphere.SetSize(Math.Max(0, sphere.Size - damage));
            if (sphere.Size <= 0)
            {
                Kill(sphere);
            }
        }

        public void IncreaseSize(SphereObjectData sphere, float size)
        {
            sphere.SetSize(sphere.Size + size);
        }

        public void Kill(SphereObjectData sphere)
        {
            sphere.Dead = true;
            Scene.RemoveView(sphere);
        }
    }
}