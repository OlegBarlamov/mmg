using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Models.SphereObject;
using SimplePhysics2D;

namespace Omegas.Client.MacOs.Services
{
    public class MapObjectsGenerator
    {
        public OmegaGameService OmegaGameService { get; }
        public ICollisionDetector2D CollisionDetector2D { get; }
        public IRandomService RandomService { get; }

        public MapObjectsGenerator(
            [NotNull] IRandomService randomService,
            [NotNull] OmegaGameService omegaGameService,
            [NotNull] ICollisionDetector2D collisionDetector2D)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            OmegaGameService = omegaGameService ?? throw new ArgumentNullException(nameof(omegaGameService));
            CollisionDetector2D = collisionDetector2D ?? throw new ArgumentNullException(nameof(collisionDetector2D));
        }

        public PlayerData PlacePlayer1(float mapWidth, float mapHeight)
        {
            return OmegaGameService.AddPlayer(new Vector2(200, 200), SphereObjectData.GetHealthFromRadius(50f),
                PlayerIndex.One);
        }
        
        public PlayerData PlacePlayer2(float mapWidth, float mapHeight)
        {
            return OmegaGameService.AddPlayer(new Vector2(mapWidth - 200, mapHeight - 200),
                SphereObjectData.GetHealthFromRadius(50f),
                PlayerIndex.Two);
        }

        public void PlaceInitialObjects(float mapWidth, float mapHeight, params PlayerData[] players)
        {
            var allObjects = new List<SphereObjectData>(players);

            allObjects.Add(PlaceNeutral(new Vector2(mapWidth / 2, mapHeight / 2), 150f));
            
            allObjects.Add(PlaceNeutral(new Vector2(mapWidth / 2, mapHeight / 5), 50f));
            allObjects.Add(PlaceNeutral(new Vector2(mapWidth / 2, mapHeight - mapHeight / 5), 50f));
            allObjects.Add(PlaceNeutral(new Vector2(mapWidth / 5, mapHeight / 2), 50f));
            allObjects.Add(PlaceNeutral(new Vector2(mapWidth - mapWidth / 5, mapHeight / 2), 50f));
            
            // P1
            for (int i = 0; i < 2; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(), new Vector2(mapWidth / 2, mapHeight / 2),
                    20f, 30f));
            }
            for (int i = 0; i < 20; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(), new Vector2(mapWidth / 2, mapHeight / 2),
                    4f, 12f));
            }
            
            // P2
            for (int i = 0; i < 2; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(mapWidth / 2, mapHeight / 2), new Vector2(mapWidth, mapHeight), 
                    20f, 30f));
            }
            for (int i = 0; i < 20; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(mapWidth / 2, mapHeight / 2), new Vector2(mapWidth, mapHeight),
                    4f, 12f));
            }
            
            // LB
            for (int i = 0; i < 20; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(0, mapHeight / 2), new Vector2(mapWidth / 2, mapHeight),
                    8f, 30f));
            }
            
            // RT
            for (int i = 0; i < 15; i++)
            {
                allObjects.Add(PlaceRandomNeutral(allObjects, new Vector2(mapWidth / 2, 0), new Vector2(mapWidth, mapHeight / 2),
                    8f, 30f));
            }
        }
        
        private SphereObjectData PlaceRandomNeutral(IReadOnlyList<SphereObjectData> existingObjects, Vector2 minPos, Vector2 maxPos, float sizeMin, float sizeMax)
        {
            var size = RandomService.NextFloat(sizeMin, sizeMax);
            var health = SphereObjectData.GetHealthFromRadius(size);
            bool collide = true;
            Vector2 position = Vector2.Zero;
            while (collide)
            {
                position = new Vector2(
                    RandomService.NextFloat(minPos.X + size, maxPos.X - size),
                    RandomService.NextFloat(minPos.Y + size, maxPos.Y - size));
                
                var neutral = new SphereObjectData(Color.LightGray, position, health, Teams.Neutral);
                collide = IsCollide(existingObjects, neutral);
            }
                
            return PlaceNeutral(position, size);
        }

        private bool IsCollide(IReadOnlyList<SphereObjectData> existingObjects, SphereObjectData sphereObjectData)
        {
            foreach (var data in existingObjects)
            {
                var collision1 = CollisionDetector2D.GetCollision(sphereObjectData.Fixture, data.Fixture);
                bool collide = !collision1.IsEmpty;
                if (collide)
                    return true;
            }

            return false;
        }

        private SphereObjectData PlaceNeutral(Vector2 pos, float size)
        {
            var health = SphereObjectData.GetHealthFromRadius(size);
            return OmegaGameService.AddNeutral(pos, health);
        }
    }
}