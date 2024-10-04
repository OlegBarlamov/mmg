using Epic.Core;

namespace Epic.Server.Resources
{
    public class BattleDefinitionResource
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        
        public BattleDefinitionResource(IBattleDefinitionObject battleDefinitionObject)
        {
            Id = battleDefinitionObject.Id.ToString();
            Width = battleDefinitionObject.Width;
            Height = battleDefinitionObject.Height;
        }
    }
}