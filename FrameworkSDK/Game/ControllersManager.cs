using System;
using Microsoft.Xna.Framework;
using NetExtensions;

namespace FrameworkSDK.Game
{
    internal class ControllersManager : IControllersManager, IUpdatable, IDisposable
    {
        private IConcurrentList<IController> Controllers { get; } = new ConcurrentList<IController>();

        public ControllersManager()
        {
            
        }

        public void AddController(IController controller)
        {
            
        }

        public void RemoveController(IController controller)
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach (var controller in Controllers)
            {
                controller.Update(gameTime);
            }
        }

        public void Dispose()
        {

        }
    }
}