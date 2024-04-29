using System;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class View<TData, TController> : View where TController : IController
    {
        protected TController Controller => (TController) ((IView) this).Controller;
        
        protected TData DataModel => (TData)((IView) this).DataModel;

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
    }
}
