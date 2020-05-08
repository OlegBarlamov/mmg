namespace NetExtensions
{
    public interface IBaseStackApi<T>
    {
        T Pop();

        void Push(T item);
    }
}