namespace MonoGameExtensions
{
    public interface IRotatable
    {
        float Rotation { get; }

        void SetRotation(float rotation);
    }
}