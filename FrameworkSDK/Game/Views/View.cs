using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkSDK.Game.Scenes;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Views
{
    public abstract class View : IView
    {
        public void Draw(GameTime gameTime)
        {
            
        }

        public object Model { get; internal set; }

        public string Name { get; }
        public Scene OwnedScene { get; }
        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
