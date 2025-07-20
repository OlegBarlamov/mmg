namespace Epic.Core.Objects
{
    public interface IGameObject<out TEntity>
    {
        TEntity ToEntity();
    }
}
