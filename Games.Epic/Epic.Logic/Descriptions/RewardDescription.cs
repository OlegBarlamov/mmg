using System;
using System.Linq;
using Epic.Core.Services;
using Epic.Core.Services.Rewards;
using Epic.Data.GameResources;
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

        public static RewardDescription[] CreateComposite(CompositeRewardObject reward, DescriptionVisibility visibility, Guid goldResourceId)
        {
            if (reward == null)
                return new []{ new RewardDescription() } ;

            var descriptions = GetRawDescriptions(reward, visibility, goldResourceId);
            foreach (var description in descriptions)
            {
                if (!string.IsNullOrWhiteSpace(reward.CustomTitle))
                {
                    description.Name = reward.CustomTitle;
                    description.Amount = "$";
                }

                if (!string.IsNullOrWhiteSpace(reward.CustomIconUrl))
                    description.IconUrl = reward.CustomIconUrl;
            }

            return descriptions;
        }

        private static RewardDescription[] GetRawDescriptions(CompositeRewardObject reward,
            DescriptionVisibility visibility, Guid goldResourceId)
        {
            if (reward.RewardType == RewardType.None)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = "None",
                            Amount = string.Empty,
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.UnitsGain)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.UnitTypes.Select(x => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = string.Empty,
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ArmySizeDescription.MaskArmySize(reward.Amounts[i]),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.Amounts[i]),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = reward.Amounts[i].ToString(),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.ResourcesGain)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = string.Empty,
                            IconUrl = x.IconUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = MaskResourcesAmount(x, reward.Amounts[i], goldResourceId),
                            IconUrl = x.IconUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ApproximateResourcesAmount(x, reward.Amounts[i], goldResourceId),
                            IconUrl = x.IconUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = reward.Amounts[i].ToString(),
                            IconUrl = x.IconUrl ?? string.Empty,
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.UnitToBuy)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "$",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "$",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "$",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "$",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.Battle)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] {  new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return new [] {  new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = string.Empty,
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        } };
                    case DescriptionVisibility.MaskedSize:
                        return new [] { new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = ArmySizeDescription.MaskArmySize(reward.NextBattleDefinition.Units.Sum(x => x.Count)),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        } };
                    case DescriptionVisibility.ApproximateSize:
                        return new [] {  new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.NextBattleDefinition.Units.Sum(x => x.Count)),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        } };
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = "Battle: " +
                                   string.Join(',', reward.NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                            Amount = reward.NextBattleDefinition.Units.Sum(x => x.Count).ToString(),
                            IconUrl =
                                reward.NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.BattleImgUrl ??
                                string.Empty,
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            throw new NotImplementedException();
        }

        private static string MaskResourcesAmount(IGameResourceEntity resourceEntity, int amount, Guid goldResourceId)
        {
            if (resourceEntity.Id == goldResourceId)
                return MaskGoldAmount(amount);

            return MaskResourceAmount(amount);
        }
        
        private static string ApproximateResourcesAmount(IGameResourceEntity resourceEntity, int amount, Guid goldResourceId)
        {
            if (resourceEntity.Id == goldResourceId)
                return ApproximateGoldAmount(amount);

            return ApproximateResourceAmount(amount);
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