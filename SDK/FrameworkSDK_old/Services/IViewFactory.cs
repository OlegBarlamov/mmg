namespace FrameworkSDK.Services
{
	public interface IViewFactory
	{
		T CreateView<T>(object model) where T : IView;
	}
}
