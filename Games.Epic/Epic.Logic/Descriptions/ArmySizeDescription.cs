using System;
using Epic.Core.Services;
using Epic.Core.Services.Units;

namespace Epic.Logic.Descriptions
{
    public class ArmySizeDescription
    {
        public string Name { get; private set; }
        public string ArmySize { get; private set; }
        public string ThumbnailUrl { get; private set; }

        private ArmySizeDescription()
        {
        }
        
        public static ArmySizeDescription Create(IGlobalUnitObject unit, DescriptionVisibility visibility)
        {
            var description = new ArmySizeDescription();

            switch (visibility)
            {
                case DescriptionVisibility.None:
                    description.Name = "Unknown";
                    description.ThumbnailUrl = PredefinedStaticResources.QuestionIconUrl;
                    break;
                case DescriptionVisibility.TypeOnly:
                    description.Name = unit.UnitType.Name;
                    description.ThumbnailUrl = unit.UnitType.BattleImgUrl;
                    break;
                case DescriptionVisibility.MaskedSize:
                    description.Name = unit.UnitType.Name;
                    description.ThumbnailUrl = unit.UnitType.BattleImgUrl;
                    description.ArmySize = MaskArmySize(unit.Count);
                    break;
                case DescriptionVisibility.ApproximateSize:
                    description.Name = unit.UnitType.Name;
                    description.ThumbnailUrl = unit.UnitType.BattleImgUrl;
                    description.ArmySize = ApproximateArmySize(unit.Count);
                    break;
                case DescriptionVisibility.Full:
                    description.Name = unit.UnitType.Name;
                    description.ThumbnailUrl = unit.UnitType.BattleImgUrl;
                    description.ArmySize = unit.Count.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
            }
            
            return description;
        }

        public static string MaskArmySize(int count)
        {
            if (count <= 0)
                return "None";
            if (count <= 4)
                return "Few";
            if (count <= 9)
                return "Several";
            if (count <= 19)
                return "Pack";
            if (count <= 49)
                return "Lots";
            if (count <= 99)
                return "Horde";
            if (count <= 249)
                return "Throng";
            if (count <= 499)
                return "Swarm";
            if (count <= 999)
                return "Zounds";
            return "Legion";
        }
        
        public static string ApproximateArmySize(int count)
        {
            if (count <= 0) return "0";
            if (count <= 4) return "~2";      
            if (count <= 14) return "~10";      
            if (count <= 32) return "~20";    
            if (count <= 74) return "~50";    
            if (count <= 132) return "~100";    
            if (count <= 299) return "~200";  
            if (count <= 699) return "~500";  
            if (count <= 999) return "~800";  
            return ">1000";
        }
    }
}