// ReSharper disable once CheckNamespace
namespace NetExtensions.Collections
{
    public interface IBaseStackApi<T>
    {
        T Pop();

        void Push(T item);
    }
}