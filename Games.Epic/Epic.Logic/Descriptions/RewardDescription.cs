using System;
using System.Linq;
using Epic.Core.Services;
using Epic.Core.Services.Rewards;
using Epic.Data.Reward;

namespace Epic.Logic.Descriptions
{
    public class RewardDescription
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string Amount { get; set; }

        private RewardDescription()
        {
        }

        public static RewardDescription Create(CompositeRewardObject reward, DescriptionVisibility visibility, Guid goldResourceId)
        {
            if (reward == null)
                return new RewardDescription();

            var description = GetRawDescription(reward, visibility, goldResourceId);
            if (!string.IsNullOrWhiteSpace(reward.CustomTitle))
            {
                description.Name = reward.CustomTitle;
                description.Amount = "$";
            }

            if (!string.IsNullOrWhiteSpace(reward.CustomIconUrl))
                description.IconUrl = reward.CustomIconUrl;

            return description;
        }

        private static RewardDescription GetRawDescription(CompositeRewardObject reward,
            DescriptionVisibility visibility, Guid goldResourceId)
        {
            if (reward.RewardType == RewardType.None)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new RewardDescription
                        {
                            Name = "None",
                            Amount = string.Empty,
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.UnitsGain)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        };
                    case DescriptionVisibility.TypeOnly:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = string.Empty,
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.MaskedSize:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = ArmySizeDescription.MaskArmySize(reward.Amounts.Sum()),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.ApproximateSize:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.Amounts.Sum()),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.Full:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = reward.Amounts.Sum().ToString(),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.ResourcesGain)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        };
                    case DescriptionVisibility.TypeOnly:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.Resources.Select(x => x.Name)),
                            Amount = string.Empty,
                            IconUrl = reward.Resources.FirstOrDefault()?.IconUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.MaskedSize:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.Resources.Select(x => x.Name)),
                            Amount = MaskResourcesAmount(reward, goldResourceId),
                            IconUrl = reward.Resources.FirstOrDefault()?.IconUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.ApproximateSize:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.Resources.Select(x => x.Name)),
                            Amount = ApproximateResourcesAmount(reward, goldResourceId),
                            IconUrl = reward.Resources.FirstOrDefault()?.IconUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.Full:
                        return new RewardDescription
                        {
                            Name = string.Join(',', reward.Resources.Select(x => x.Name)),
                            Amount = string.Join(',', reward.Amounts.Select(x => x.ToString())),
                            IconUrl = reward.Resources.FirstOrDefault()?.IconUrl ?? string.Empty,
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.UnitToBuy)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        };
                    case DescriptionVisibility.TypeOnly:
                        return new RewardDescription
                        {
                            Name = "Buy: " + string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = string.Empty,
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.MaskedSize:
                        return new RewardDescription
                        {
                            Name = "Buy: " + string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = ArmySizeDescription.MaskArmySize(reward.Amounts.Sum()),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.ApproximateSize:
                        return new RewardDescription
                        {
                            Name = "Buy: " + string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.Amounts.Sum()),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    case DescriptionVisibility.Full:
                        return new RewardDescription
                        {
                            Name = "Buy: " + string.Join(',', reward.UnitTypes.Select(x => x.Name)),
                            Amount = reward.Amounts.Sum().ToString(),
                            IconUrl = reward.UnitTypes.FirstOrDefault()?.BattleImgUrl ?? string.Empty,
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.Battle)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        };
                    case DescriptionVisibility.TypeOnly:
                        return new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = string.Empty,
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        };
                    case DescriptionVisibility.MaskedSize:
                        return new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = ArmySizeDescription.MaskArmySize(reward.NextBattleDefinition.Units.Sum(x => x.Count)),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        };
                    case DescriptionVisibility.ApproximateSize:
                        return new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.NextBattleDefinition.Units.Sum(x => x.Count)),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        };
                    case DescriptionVisibility.Full:
                        return new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = reward.NextBattleDefinition.Units.Sum(x => x.Count).ToString(),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            throw new NotImplementedException();
        }

        private static string MaskResourcesAmount(IRewardObject rewardObject, Guid goldResourceId)
        {
            if (rewardObject.Ids.Contains(goldResourceId))
                return MaskGoldAmount(rewardObject.Amounts.Sum());

            return MaskResourceAmount(rewardObject.Amounts.Sum());
        }
        
        private static string ApproximateResourcesAmount(IRewardObject rewardObject, Guid goldResourceId)
        {
            if (rewardObject.Ids.Contains(goldResourceId))
                return ApproximateGoldAmount(rewardObject.Amounts.Sum());

            return ApproximateResourceAmount(rewardObject.Amounts.Sum());
        }

        private static string MaskGoldAmount(int amount)
        {
            if (amount <= 0)         return "No";
            if (amount <= 19)        return "Handful";
            if (amount <= 99)        return "Pouch";
            if (amount <= 299)       return "Purse";
            if (amount <= 799)       return "Stack";
            if (amount <= 1999)      return "Small coffer";
            if (amount <= 4999)      return "Chest";
            if (amount <= 14999)     return "Vault";
            if (amount <= 39999)     return "Treasure trove";
            if (amount <= 99999)     return "King's ransom";
            return "Legendary fortune"; // 100,000+
        }
        
        private static string MaskResourceAmount(int amount)
        {
            if (amount <= 0)    return "None";
            if (amount <= 1)    return "Single";
            if (amount <= 2)    return "Pair";
            if (amount <= 4)    return "Cluster";
            if (amount <= 7)    return "Set";
            if (amount <= 14)   return "Pocketful";
            if (amount <= 24)   return "Handful";
            if (amount <= 39)   return "Pouch";
            if (amount <= 59)   return "Box";
            if (amount <= 98)   return "Casket";
            return "Treasure"; // 99+
        }
        
        private static string ApproximateGoldAmount(int amount)
        {
            if (amount <= 0)         return "0";
            if (amount <= 19)        return "~10";
            if (amount <= 99)        return "~50";
            if (amount <= 299)       return "~200";
            if (amount <= 799)       return "~500";
            if (amount <= 1999)      return "~1,200";
            if (amount <= 4999)      return "~3,000";
            if (amount <= 14999)     return "~8,000";
            if (amount <= 39999)     return "~25,000";
            if (amount <= 99999)     return "~70,000";
            return "~150,000"; // 100,000+
        }
        
        private static string ApproximateResourceAmount(int amount)
        {
            if (amount <= 0)    return "0";
            if (amount <= 1)    return "~1";
            if (amount <= 2)    return "~2";
            if (amount <= 4)    return "~3";
            if (amount <= 7)    return "~6";
            if (amount <= 14)   return "~10";
            if (amount <= 24)   return "~20";
            if (amount <= 39)   return "~30";
            if (amount <= 59)   return "~50";
            if (amount <= 98)   return "~75";
            return "~120"; // 99+
        }
    }
}