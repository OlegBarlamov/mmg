using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemAsLightPointDetailsGenerator : IDetailsGenerator<StarSystemAsLightPoint>
    {
        public void Generate(StarSystemAsLightPoint target)
        {
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarSystemAsLightPoint)target);
        }
    }
}
