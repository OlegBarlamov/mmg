using System;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class Controller<TModel> : Controller
    {
        protected TModel Model => (TModel)((IController)this).Model;

        protected Controller()
        {
        }

        protected Controller([NotNull] string name) : base(name)
        {
        }

        protected sealed override void SetModel([NotNull] object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (!(model is TModel))
                throw new IncompatibleModelTypeException(string.Format(Strings.Exceptions.Mapping.IncompatibleModelType, model.GetType(), typeof(TModel)));

            base.SetModel(model);
        }
    }
}
