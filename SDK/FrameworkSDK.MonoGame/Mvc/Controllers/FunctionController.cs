using System;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
    public delegate void ControllerTickDelegate<TModel>(FunctionController<TModel> controller, GameTime gameTime);

    public class FunctionController<TModel> : Controller<TModel>
    {
        public TModel DataModel => base.Model;
        public ControllerTickDelegate<TModel> Tick { get; }
        
        public FunctionController(ControllerTickDelegate<TModel> tick)
        {
            Tick = tick ?? throw new ArgumentNullException(nameof(tick));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            Tick.Invoke(this, gameTime);
        }
    }
}