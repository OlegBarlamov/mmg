using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics._2D.BodyTypes;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Helpers;
using Omegas.Client.MacOs.Services;
using SimplePhysics2D.Fixtures;

namespace Omegas.Client.MacOs.Models.SphereObject
{
    public class SphereObjectData : IColliderBody2D
    {
        public const float MinHealth = MathHelper.Pi;
        public Teams Team { get; }
        
        public Color Color { get; }
        
        public bool Dead { get; set; }

        public Vector2 Position { get; private set; }

        public virtual void SetPosition(Vector2 position)
        {
            if (Double.IsNaN(position.X) || Double.IsInfinity(position.X))
            {
                return;
            }

            Position = position;
            ViewModel.BoundingBox = GetBoundingBox();
            _fixture.Center = Position;
        }

        public float Rotation { get; private set; }
        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }
        
        public float Health { get; private set; }

        public void SetHealth(float health)
        {
            Health = health;
            var radius = GetRadiusFromHealth(health);
            SetSize(radius);
        }

        public float Size { get; private set; } = 1f;

        private void SetSize(float size)
        {
            Size = size;
            ViewModel.BoundingBox = GetBoundingBox();
            _fixture.Radius = size;
            _parameters.Mass = GetMassFromHealth(Health);
        }

        public IPhysicsBody2DParameters Parameters => _parameters;
        private readonly DynamicBody2DParameters _parameters  = new DynamicBody2DParameters
        {
            FrictionFactor = 0.1f
        };
        
        public IScene2DPhysics Scene { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public ICollection<IForce2D> ActiveForces { get; } = new List<IForce2D>();
        public bool NoClipMode { get; set; }
        public IFixture2D Fixture => _fixture;
        
        public List<SphereObjectData> CollidingSpheres { get; } = new List<SphereObjectData>();
        
        public bool OnCollision(IColliderBody2D body)
        {
            if (body is SphereObjectData sphereObjectData)
            {
                if (!CollidingSpheres.Contains(sphereObjectData))
                    CollidingSpheres.Add(sphereObjectData);
            }

            if (body is MapBoundaries map)
            {
                return map.OnCollision(this);
            }

            return false;
        }

        private readonly CircleFixture _fixture;
        
        public SphereObjectViewModel ViewModel { get; }

        public SphereObjectData(Color color, Vector2 position, float health, Teams team)
        {
            Position = position;
            Color = color;
            Team = team;
            
            _fixture = new CircleFixture(this, Position, Size);
            ViewModel  = new SphereObjectViewModel
            {
                BoundingBox = GetBoundingBox(),
                Color = Color,
            };

            SetHealth(health);
        }

        public static float GetRadiusFromHealth(float health)
        {
            return MathExtended.Sqrt(health / MathHelper.Pi);
        }

        public static float GetMassFromHealth(float health)
        {
            return health * 0.01f;
        }

        public static float GetHealthFromRadius(float radius)
        {
            return MathExtended.Sqr(radius) * MathHelper.Pi;
        }

        protected RectangleF GetBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position,
                Size);
        }
    }
}