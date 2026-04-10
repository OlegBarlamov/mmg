using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World;

namespace Atom.Client.Controllers
{
    public interface IWrappedObjectsController
    {
        event Action<IWrappedDetails> ObjectRevealed;
        event Action<IReadOnlyList<IWrappedDetails>> ObjectsRevealedBatch;
        event Action<IReadOnlyList<IWrappedDetails>> ObjectsHidden;

        void Update(Vector3 playerPosition, GameTime gameTime);
        
        void AddWrappedObject(IWrappedDetails wrappedDetails);
        void AddUnwrappedObject(IWrappedDetails wrappedDetails);
        void RemoveWrappedObject(IWrappedDetails wrappedDetails);
        void RemoveUnwrappedObject(IWrappedDetails wrappedDetails);

        bool IsObjectRevealed(IWrappedDetails details);
    }
}