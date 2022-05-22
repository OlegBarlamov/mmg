using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
    public class EmptyMvcStrategyService : IMvcStrategyService
    {
        public IMvcComponentGroup ResolveByModel(object model)
        {
            return new MvcComponentGroup
            {
                Model = model
            };
        }

        public IMvcComponentGroup ResolveByController(IController controller)
        {
            return new MvcComponentGroup
            {
                Controller = controller
            };
        }

        public IMvcComponentGroup ResolveByView(IView view)
        {
            return new MvcComponentGroup
            {
                View = view
            };
        }

        public IMvcSchemeValidateResult ValidateByModel(object model)
        {
            return new MvcSchemeValidateResult
            {
                IsModelExist = true
            };
        }

        public IMvcSchemeValidateResult ValidateByController(IController controller)
        {
            return new MvcSchemeValidateResult
            {
                IsControllerExist = true
            };
        }

        public IMvcSchemeValidateResult ValidateByView(IView view)
        {
            return new MvcSchemeValidateResult
            {
                IsViewExist = true
            };
        }
    }
}