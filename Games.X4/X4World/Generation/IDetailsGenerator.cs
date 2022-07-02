namespace X4World.Generation
{
    public interface IDetailsGenerator
    {
        void Generate(IGeneratorTarget target);
    }
    
    public interface IDetailsGenerator<in TGeneratorTarget> : IDetailsGenerator where TGeneratorTarget : IWrappedDetails
    {
        void Generate(TGeneratorTarget target);
    }
}