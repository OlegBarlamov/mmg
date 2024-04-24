using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Physics2D;
using NetExtensions.Collections;

namespace SimplePhysics2D.Spaces
{
    public class HeapCollidersSpace2D : ICollidersSpace2D
    {
        private readonly Heap<IColliderBody2D> _heap = new Heap<IColliderBody2D>();
        
        public void AddBody(IColliderBody2D body)
        {
            _heap.Add(body);
        }

        public void RemoveBody(IColliderBody2D body)
        {
            _heap.Remove(body);
        }

        public IEnumerable<IColliderBody2D> GetCollisionCandidates(IColliderBody2D body)
        {
            return _heap.Where(x => x != body);
        }
    }
}