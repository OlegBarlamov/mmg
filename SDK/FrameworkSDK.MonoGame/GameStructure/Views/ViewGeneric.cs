using System;
using FrameworkSDK.Localization;
using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Mapping;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Views
{
    public abstract class View<TData, TController> : View where TController : IController
    {
        protected TData DataModel => (TData)((IView) this).DataModel;

        protected TController Controller => (TController) ((IView) this).Controller;

        protected View()
        {
        }

        protected View([NotNull] string name) : base(name)
        {
        }

        protected sealed override void SetDataModel([NotNull] object dataModel)
        {
            if (dataModel == null) throw new ArgumentNullException(nameof(dataModel));

            if (!(dataModel is TData))
                throw new IncompatibleModelTypeException(string.Format(Strings.Exceptions.Mapping.IncompatibleModelType, dataModel.GetType(), typeof(TData)));

            base.SetDataModel(dataModel);
        }

        protected sealed override void SetController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            if (!(controller is TController))
                throw new IncompatibleControllerTypeException(string.Format(Strings.Exceptions.Mapping.IncompatibleControllerType, controller.GetType(), typeof(TController)));

            base.SetController(controller);
        }
    }

    public abstract class View<TData> : View<TData, EmptyController>
    {
        protected View()
        {
        }

        protected View([NotNull] string name) : base(name)
        {
        }
    }
}
