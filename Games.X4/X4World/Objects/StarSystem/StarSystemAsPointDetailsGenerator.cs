using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemAsPointDetailsGenerator : IDetailsGenerator<StarSystemAsPoint>
    {
        public void Generate(StarSystemAsPoint target)
        {
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarSystemAsPoint)target);
        }
    }
}
