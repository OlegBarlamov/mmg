namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IMvcMappingResolver
    {
        IMvcComponentGroup ResolveByModel(object model);
        IMvcComponentGroup ResolveByController(IController controller);
        IMvcComponentGroup ResolveByView(IView view);

        IMvcSchemeValidateResult ValidateByModel(object model);
        IMvcSchemeValidateResult ValidateByController(IController controller);
        IMvcSchemeValidateResult ValidateByView(IView view);
    }
}