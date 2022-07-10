using System;
using Microsoft.Xna.Framework;
using X4World;

namespace Atom.Client.Controllers
{
    public interface IWrappedObjectsController
    {
        event Action<IWrappedDetails> ObjectRevealed;
        event Action<IWrappedDetails> ObjectHidden;

        void Update(Vector3 playerPosition, GameTime gameTime);
        
        void AddWrappedObject(IWrappedDetails wrappedDetails);
        void AddUnwrappedObject(IWrappedDetails wrappedDetails);
        void RemoveWrappedObject(IWrappedDetails wrappedDetails);
        void RemoveUnwrappedObject(IWrappedDetails wrappedDetails);
    }
}