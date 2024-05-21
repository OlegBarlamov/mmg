using System;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Helpers;
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

        public SphereObjectData AddSphereObject(Color color, Vector2 position, float health, Teams team)
        {
            var data = new SphereObjectData(color, position, health, team);

            Scene.AddView(data);

            return data;
        }

        public SphereObjectData CreateBulletWorkpiece(PlayerData player, Vector2 originNormal)
        {
            var bulletHealth = Math.Max(SphereObjectData.MinHealth, player.Health * 0.005f);
            var bulletSize = SphereObjectData.GetRadiusFromHealth(bulletHealth);
            var bulletPosition = player.Position + originNormal * (player.Size + bulletSize);
            
            var data = AddSphereObject(player.Color, bulletPosition, bulletHealth, player.Team);
            data.NoClipMode = true;
            
            TakeDamage(player, bulletHealth);

            return data;
        }

        public bool CanProduceBullets(PlayerData playerData)
        {
            return playerData.Health > SphereObjectData.MinHealth * 100;
        }

        public void CancelBullet(PlayerData player, SphereObjectData bullet)
        {
            IncreaseHealth(player, bullet.Health);
            Kill(bullet);
        }

        public void ReleaseBullet(PlayerData player, SphereObjectData bullet, Vector2 origin, Vector2 originNormal)
        {
            var impulseVelocity = origin * 30f;
            var targetVelocity = player.Velocity + impulseVelocity;
            if (player.Velocity != Vector2.Zero && Vector2.Dot(Vector2.Normalize(player.Velocity), originNormal) > 0)
                bullet.SetPosition(bullet.Position + player.Velocity);
            
            bullet.NoClipMode = false;
            
            Scene.Physics2D.ApplyImpulse(bullet, targetVelocity * bullet.Parameters.Mass);
            Scene.Physics2D.ApplyImpulse(player, -1 * impulseVelocity * bullet.Parameters.Mass * 2);
        }

        public void FillBullet(PlayerData playerData, SphereObjectData bullet, float factor, GameTime gameTime)
        {
            var fillingSpeed = factor * gameTime.ElapsedGameTime.Milliseconds * 0.1f;
            TakeDamage(playerData, fillingSpeed);
            IncreaseHealth(bullet, fillingSpeed);
        }

        public void HandleConsumption(SphereObjectData sphereA, SphereObjectData sphereB, GameTime gameTime)
        {
            if (sphereA.Team.IsAllyWith(sphereB.Team))
                return;
            
            var biggerSphere = sphereA.Size > sphereB.Size ? sphereA : sphereB;
            var smallerSphere = sphereA.Size > sphereB.Size ? sphereB : sphereA;

            var oldGap = Vector2.Distance(biggerSphere.Position, smallerSphere.Position) - (biggerSphere.Size + smallerSphere.Size);
            
            if (sphereA.Team.IsNeutralWith(sphereB.Team))
            {
                var healthConsumption = 1f * gameTime.ElapsedGameTime.Milliseconds * 1f;
                IncreaseHealth(biggerSphere, healthConsumption);
                TakeDamage(smallerSphere, healthConsumption);
            }
            if (sphereA.Team.IsSelf(sphereB.Team))
            {
                var healthConsumption = 1f * gameTime.ElapsedGameTime.Milliseconds * 0.5f;
                if (sphereA is PlayerData)
                {
                    IncreaseHealth(sphereA, healthConsumption);
                    TakeDamage(sphereB, healthConsumption);
                }
                else if (sphereB is PlayerData)
                {
                    IncreaseHealth(sphereB, healthConsumption);
                    TakeDamage(sphereA, healthConsumption);
                }
                else
                {
                    IncreaseHealth(biggerSphere, healthConsumption);
                    TakeDamage(smallerSphere, healthConsumption);   
                }
            }
            if (sphereA.Team.IsEnemyWith(sphereB.Team))
            {
                var radiusConsumption = 1f * gameTime.ElapsedGameTime.Milliseconds * 0.005f;
                var biggerSphereHealthDamage = biggerSphere.Health - SphereObjectData.GetHealthFromRadius(biggerSphere.Size - radiusConsumption);
                var smallerSphereHealthDamage = smallerSphere.Health - SphereObjectData.GetHealthFromRadius(smallerSphere.Size - radiusConsumption);
                TakeDamage(biggerSphere, biggerSphereHealthDamage);
                TakeDamage(smallerSphere, smallerSphereHealthDamage);
            }
            
            var newGap = Vector2.Distance(biggerSphere.Position, smallerSphere.Position) - (biggerSphere.Size + smallerSphere.Size);
            var gapsDifference = newGap - oldGap;
            if (gapsDifference > 0)
            {
                var normal = Vector2.Normalize(biggerSphere.Position - smallerSphere.Position);
                smallerSphere.SetPosition(smallerSphere.Position + normal * gapsDifference * 1.25f);
            }
        }

        public void JumpAction(PlayerData player, Vector2 originNormal)
        {
            var bulletHealth = Math.Max(player.Health * 0.10f, SphereObjectData.MinHealth);
            var bulletSize = SphereObjectData.GetRadiusFromHealth(bulletHealth);
            var bulletPosition = player.Position + originNormal * (player.Size + bulletSize);
            
            AddSphereObject(player.Color, bulletPosition, bulletHealth, player.Team);
            
            var playerOldMass = player.Parameters.Mass;

            TakeDamage(player, bulletHealth);
            
            Scene.Physics2D.ApplyImpulse(player, -1 * player.Velocity * player.Parameters.Mass -1 * originNormal * playerOldMass * player.Parameters.Mass);
        }

        public void TakeDamage(SphereObjectData sphere, float damage)
        {
            sphere.SetHealth(Math.Max(0, sphere.Health - damage));
            if (sphere.Health < SphereObjectData.MinHealth)
            {
                Kill(sphere);
            }
        }

        public void IncreaseHealth(SphereObjectData sphere, float health)
        {
            sphere.SetHealth(sphere.Health + health);
        }

        public void Kill(SphereObjectData sphere)
        {
            sphere.Dead = true;
            Scene.RemoveView(sphere);
        }
    }
}