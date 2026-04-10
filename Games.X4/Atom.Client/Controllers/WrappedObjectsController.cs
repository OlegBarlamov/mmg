using System;
using System.Collections.Generic;
using Atom.Client.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using X4World;

namespace Atom.Client.Controllers
{
    class WrappedObjectsController : IWrappedObjectsController
    {
        public event Action<IWrappedDetails> ObjectRevealed;
        public event Action<IReadOnlyList<IWrappedDetails>> ObjectsRevealedBatch;
        public event Action<IReadOnlyList<IWrappedDetails>> ObjectsHidden;

        private readonly IDictionary<string, IWrappedDetails> _unwrappedHashtable = new Dictionary<string, IWrappedDetails>();
        private readonly List<IWrappedDetails> _unwrappedObjects = new List<IWrappedDetails>(); 
        private readonly List<IWrappedDetails> _wrappedObjectsWithoutParent = new List<IWrappedDetails>();

        private IDetailsGeneratorProvider GeneratorProvider { get; }
        
        private Vector3? _lastPlayerPosition;

        private readonly List<IWrappedDetails> _objectsToWrap = new List<IWrappedDetails>();
        private readonly Queue<IWrappedDetails> _objectsToUnwrap = new Queue<IWrappedDetails>();
        private readonly List<IWrappedDetails> _objectsToHideBatch = new List<IWrappedDetails>();

        public WrappedObjectsController([NotNull] IDetailsGeneratorProvider generatorProvider)
        {
            GeneratorProvider = generatorProvider ?? throw new ArgumentNullException(nameof(generatorProvider));
        }
        
        public void Update(Vector3 playerPosition, GameTime gameTime)
        {
            if (playerPosition == _lastPlayerPosition)
                return;

            _lastPlayerPosition = playerPosition;
            foreach (var unwrappedObject in _unwrappedObjects)
            {
                var unwrapDist = unwrappedObject.DistanceToUnwrapDetails;
                var worldPos = unwrappedObject.GetWorldPosition();
                var distSq = (worldPos - playerPosition).LengthSquared();
                if (distSq > unwrapDist * unwrapDist)
                {
                    _objectsToWrap.Add(unwrappedObject);
                }
                else
                {
                    var localPlayerPosition = playerPosition - worldPos;
                    var potentialObjectsToUnwrap = unwrappedObject.Details.GetInRadius(localPlayerPosition, unwrapDist / 2);
                    foreach (var potentialObjectToUnwrap in potentialObjectsToUnwrap)
                    {
                        var childDist = potentialObjectToUnwrap.DistanceToUnwrapDetails;
                        var childDistSq = (potentialObjectToUnwrap.Position - localPlayerPosition).LengthSquared();
                        if (!_unwrappedHashtable.ContainsKey(potentialObjectToUnwrap.Name) &&
                            childDistSq < childDist * childDist)
                        {
                            _objectsToUnwrap.Enqueue(potentialObjectToUnwrap);
                        }
                    }
                }
            }

            for (var index = 0; index < _wrappedObjectsWithoutParent.Count; index++)
            {
                var wrappedObject = _wrappedObjectsWithoutParent[index];
                var worldPosition = wrappedObject.GetWorldPosition();
                var wrapDist = wrappedObject.DistanceToUnwrapDetails;
                var rootDistSq = (playerPosition - worldPosition).LengthSquared();
                if (rootDistSq < wrapDist * wrapDist)
                {
                    _wrappedObjectsWithoutParent.RemoveAt(index);
                    _objectsToUnwrap.Enqueue(wrappedObject);
                    index--;
                }
            }

            _objectsToWrap.Sort((a, b) => GetDepth(a).CompareTo(GetDepth(b)));
            foreach (var objectToWrap in _objectsToWrap)
            {
                if (!_unwrappedHashtable.ContainsKey(objectToWrap.Name))
                    continue;

                _unwrappedHashtable.Remove(objectToWrap.Name);
                _unwrappedObjects.SwapAndPopRemove(objectToWrap);
                var parentAlive = objectToWrap.Parent == null || _unwrappedHashtable.ContainsKey(objectToWrap.Parent.Name);
                if (parentAlive)
                    ObjectRevealed?.Invoke(objectToWrap);

                CollectObjectsToHide(objectToWrap, _objectsToHideBatch);
                if (objectToWrap.Parent == null)
                    _wrappedObjectsWithoutParent.Add(objectToWrap);
            }
            _objectsToWrap.Clear();

            while (_objectsToUnwrap.Count > 0)
            {
                var objectToUnwrap = _objectsToUnwrap.Dequeue();
                if (objectToUnwrap.Parent != null && !_unwrappedHashtable.ContainsKey(objectToUnwrap.Parent.Name))
                    continue;
                _unwrappedHashtable.Add(objectToUnwrap.Name, objectToUnwrap);
                _unwrappedObjects.Add(objectToUnwrap);
                if (!objectToUnwrap.IsDetailsGenerated)
                {
                    var generator = GeneratorProvider.GetGenerator(objectToUnwrap);
                    generator.Generate(objectToUnwrap);
                }

                var batchDetails = new List<IWrappedDetails>(objectToUnwrap.Details.Count);
                foreach (var detail in objectToUnwrap.Details)
                    batchDetails.Add(detail);
                batchDetails.Sort((a, b) =>
                    (a.GetWorldPosition() - playerPosition).LengthSquared()
                        .CompareTo((b.GetWorldPosition() - playerPosition).LengthSquared()));
                ObjectsRevealedBatch?.Invoke(batchDetails);

                _objectsToHideBatch.Add(objectToUnwrap);
            }

            if (_objectsToHideBatch.Count > 0)
            {
                ObjectsHidden?.Invoke(new List<IWrappedDetails>(_objectsToHideBatch));
                _objectsToHideBatch.Clear();
            }
        }

        public void AddWrappedObject(IWrappedDetails wrappedDetails)
        {
            if (wrappedDetails.Parent != null)
            {
                AddUnwrappedObject(wrappedDetails.Parent);
            }
            else
            {
                _wrappedObjectsWithoutParent.Add(wrappedDetails);
                ObjectRevealed?.Invoke(wrappedDetails);
            }
        }
        
        public void RemoveWrappedObject(IWrappedDetails wrappedDetails)
        {
            _wrappedObjectsWithoutParent.Remove(wrappedDetails);
            ObjectsHidden?.Invoke(new[] { wrappedDetails });
        }

        public void AddUnwrappedObject(IWrappedDetails wrappedDetails)
        {
            _unwrappedObjects.Add(wrappedDetails);
            foreach (var detail in wrappedDetails.Details)
            {
                ObjectRevealed?.Invoke(detail);
            }
        }

        public bool IsObjectRevealed(IWrappedDetails details)
        {
            if (_unwrappedHashtable.ContainsKey(details.Name))
                return false;
            if (details.Parent != null)
                return _unwrappedHashtable.ContainsKey(details.Parent.Name);
            return true;
        }

        public bool IsObjectUnwrapped(IWrappedDetails details)
        {
            return _unwrappedHashtable.ContainsKey(details.Name);
        }

        public void RemoveUnwrappedObject(IWrappedDetails wrappedDetails)
        {
            _unwrappedHashtable.Remove(wrappedDetails.Name);
            _unwrappedObjects.SwapAndPopRemove(wrappedDetails);
            var batch = new List<IWrappedDetails>();
            CollectObjectsToHide(wrappedDetails, batch);
            if (batch.Count > 0)
                ObjectsHidden?.Invoke(batch);
        }

        private static int GetDepth(IWrappedDetails details)
        {
            int depth = 0;
            var current = details.Parent;
            while (current != null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }

        private void CollectObjectsToHide(IWrappedDetails wrappedDetails, List<IWrappedDetails> batch)
        {
            foreach (var detail in wrappedDetails.Details)
            {
                if (_unwrappedHashtable.ContainsKey(detail.Name))
                {
                    _unwrappedHashtable.Remove(detail.Name);
                    _unwrappedObjects.SwapAndPopRemove(detail);
                    CollectObjectsToHide(detail, batch);
                }
                else
                {
                    batch.Add(detail);
                }
            }
        }
    }
}