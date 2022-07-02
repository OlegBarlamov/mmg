using System;
using System.Collections.Generic;
using Atom.Client.MacOS.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World;

namespace Atom.Client.MacOS.Controllers
{
    class WrappedObjectsController : IWrappedObjectsController
    {
        public IDetailsGeneratorProvider GeneratorProvider { get; }
        public event Action<IWrappedDetails> ObjectRevealed;
        public event Action<IWrappedDetails> ObjectHidden;

        private readonly IDictionary<string, IWrappedDetails> _unwrappedHashtable = new Dictionary<string, IWrappedDetails>();
        private readonly List<IWrappedDetails> _unwrappedObjects = new List<IWrappedDetails>(); 
        //private readonly List<IWrappedDetails> _wrappedObjects = new List<IWrappedDetails>();

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

            while (_objectsToWrap.Count > 0)
            {
                var objectToWrap = _objectsToWrap.Dequeue();
                _unwrappedHashtable.Remove(objectToWrap.Name);
                _unwrappedObjects.Remove(objectToWrap);
                //_wrappedObjects.Add(objectToWrap);
                ObjectRevealed?.Invoke(objectToWrap);
                foreach (var detail in objectToWrap.Details)
                {
                    ObjectHidden?.Invoke(detail);
                }
            }

            while (_objectsToUnwrap.Count > 0)
            {
                var objectToUnwrap = _objectsToUnwrap.Dequeue();
                _unwrappedHashtable.Add(objectToUnwrap.Name, objectToUnwrap);
                //_wrappedObjects.Remove(objectToUnwrap);
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
            //_wrappedObjects.Add(wrappedDetails);
            ObjectRevealed?.Invoke(wrappedDetails);
        }

        public void AddUnwrappedObject(IWrappedDetails wrappedDetails)
        {
            _unwrappedObjects.Add(wrappedDetails);
            foreach (var detail in wrappedDetails.Details)
            {
                //_wrappedObjects.Add(detail);
                ObjectRevealed?.Invoke(detail);
            }
        }

        public void RemoveWrappedObject(IWrappedDetails wrappedDetails)
        {
            //_wrappedObjects.Remove(wrappedDetails);
            ObjectHidden?.Invoke(wrappedDetails);
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