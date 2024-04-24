using System;
using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;

namespace SimplePhysics2D
{
    public struct Collision2D
    {
        public static Collision2D Empty { get; } = new Collision2D(true);

        public readonly bool IsEmpty;
        
        public Vector2[] Points;

        public IFixture2D FixtureA;
        
        public IFixture2D FixtureB;

        private Collision2D(bool isEmpty)
        {
            IsEmpty = isEmpty;
            Points = Array.Empty<Vector2>();
            FixtureA = null;
            FixtureB = null;
        }

        internal Collision2D(IFixture2D fixtureA, IFixture2D fixtureB, Vector2[] points)
        {
            IsEmpty = false;
            FixtureA = fixtureA;
            FixtureB = fixtureB;
            Points = points;
        }
    }
}