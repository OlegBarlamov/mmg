using System;
using System.Collections.Generic;
using Atom.Client.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World;

namespace Atom.Client.Controllers
{
    class WrappedObjectsController : IWrappedObjectsController
    {
        public event Action<IWrappedDetails> ObjectRevealed;
        public event Action<IWrappedDetails> ObjectHidden;

        private readonly IDictionary<string, IWrappedDetails> _unwrappedHashtable = new Dictionary<string, IWrappedDetails>();
        private readonly List<IWrappedDetails> _unwrappedObjects = new List<IWrappedDetails>(); 
        private readonly List<IWrappedDetails> _wrappedObjectsWithoutParent = new List<IWrappedDetails>();

        private IDetailsGeneratorProvider GeneratorProvider { get; }
        
        private Vector3? _lastPlayerPosition;

        private readonly Queue<IWrappedDetails> _objectsToWrap = new Queue<IWrappedDetails>();
        private readonly Queue<IWrappedDetails> _objectsToUnwrap = new Queue<IWrappedDetails>();

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
                if ((unwrappedObject.GetWorldPosition() - playerPosition).Length() >
                    unwrappedObject.DistanceToUnwrapDetails)
                {
                    _objectsToWrap.Enqueue(unwrappedObject);
                }
                else
                {
                    var localPlayerPosition = playerPosition - unwrappedObject.GetWorldPosition();
                    var potentialObjectsToUnwrap = unwrappedObject.Details.GetInRadius(localPlayerPosition, unwrappedObject.DistanceToUnwrapDetails / 2);
                    foreach (var potentialObjectToUnwrap in potentialObjectsToUnwrap)
                    {
                        if (!_unwrappedHashtable.ContainsKey(potentialObjectToUnwrap.Name) && (potentialObjectToUnwrap.Position - localPlayerPosition).Length() <
                            potentialObjectToUnwrap.DistanceToUnwrapDetails)
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
                if ((playerPosition - worldPosition).Length() < wrappedObject.DistanceToUnwrapDetails)
                {
                    _wrappedObjectsWithoutParent.Remove(wrappedObject);
                    _objectsToUnwrap.Enqueue(wrappedObject);
                    index--;
                }
            }

            while (_objectsToWrap.Count > 0)
            {
                var objectToWrap = _objectsToWrap.Dequeue();
                _unwrappedHashtable.Remove(objectToWrap.Name);
                _unwrappedObjects.Remove(objectToWrap);
                ObjectRevealed?.Invoke(objectToWrap);
                foreach (var detail in objectToWrap.Details)
                {
                    ObjectHidden?.Invoke(detail);
                }
                if (objectToWrap.Parent == null)
                    _wrappedObjectsWithoutParent.Add(objectToWrap);
            }

            while (_objectsToUnwrap.Count > 0)
            {
                var objectToUnwrap = _objectsToUnwrap.Dequeue();
                _unwrappedHashtable.Add(objectToUnwrap.Name, objectToUnwrap);
                _unwrappedObjects.Add(objectToUnwrap);
                if (!objectToUnwrap.IsDetailsGenerated)
                {
                    var generator = GeneratorProvider.GetGenerator(objectToUnwrap);
                    generator.Generate(objectToUnwrap);
                }
                foreach (var detail in objectToUnwrap.Details)
                {
                    ObjectRevealed?.Invoke(detail);
                }
                ObjectHidden?.Invoke(objectToUnwrap);
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
            ObjectHidden?.Invoke(wrappedDetails);
        }

        public void AddUnwrappedObject(IWrappedDetails wrappedDetails)
        {
            _unwrappedObjects.Add(wrappedDetails);
            foreach (var detail in wrappedDetails.Details)
            {
                ObjectRevealed?.Invoke(detail);
            }
        }

        public void RemoveUnwrappedObject(IWrappedDetails wrappedDetails)
        {
            _unwrappedHashtable.Remove(wrappedDetails.Name);
            _unwrappedObjects.Remove(wrappedDetails);
            foreach (var detail in wrappedDetails.Details)
            {
                if (_unwrappedHashtable.ContainsKey(detail.Name))
                {
                    RemoveUnwrappedObject(detail);
                }
                else
                {
                    ObjectHidden?.Invoke(detail);
                }
            }
        }
    }
}