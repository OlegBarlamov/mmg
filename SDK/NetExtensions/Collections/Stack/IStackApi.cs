// ReSharper disable once CheckNamespace
namespace NetExtensions.Collections
{
    public interface IStackApi<T> : IBaseStackApi<T>
    {
        T Peek();
    }
}