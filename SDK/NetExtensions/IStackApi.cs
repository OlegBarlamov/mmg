namespace NetExtensions
{
    public interface IStackApi<T> : IBaseStackApi<T>
    {
        T Peek();
    }
}