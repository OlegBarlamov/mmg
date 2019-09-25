using System;
using FrameworkSDK.Localization;
using FrameworkSDK.MonoGame.Mvc;
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

    [Obsolete("Наверное не нужно использовать. Плохой паттерн")]
    public abstract class Controller<TModel, TView> : Controller
    {
        protected TModel Model => (TModel)((IController)this).Model;

        protected TView View => (TView)((IController)this).View;

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

        protected sealed override void SetView([NotNull] IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            if (!(view is TView))
                throw new IncompatibleViewTypeException(string.Format(Strings.Exceptions.Mapping.IncompatibleViewType, view.GetType(), typeof(TView)));

            base.SetView(view);
        }
    }
}
