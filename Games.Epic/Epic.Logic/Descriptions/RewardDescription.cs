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
        public string Tooltip { get; set; }
        public bool IsNextStage { get; set; }

        private RewardDescription()
        {
        }

        public static RewardDescription[] CreateComposite(CompositeRewardObject reward, DescriptionVisibility visibility, Guid goldResourceId, double? rewardFactor = null)
        {
            if (reward == null)
                return new []{ new RewardDescription() } ;

            if (!string.IsNullOrWhiteSpace(reward.Description) && !string.IsNullOrWhiteSpace(reward.IconUrl) && !string.IsNullOrWhiteSpace(reward.Title))
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = reward.Title,
                            Amount = string.Empty,
                            IconUrl = reward.IconUrl,
                            Tooltip = reward.Description,
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }
            
            var descriptions = GetRawDescriptions(reward, visibility, goldResourceId, rewardFactor);
            foreach (var description in descriptions)
            {
                if (!string.IsNullOrWhiteSpace(reward.Title))
                {
                    description.Name = reward.Title;
                }

                if (!string.IsNullOrWhiteSpace(reward.IconUrl))
                {
                    description.IconUrl = reward.IconUrl;
                }
                
                if (!string.IsNullOrWhiteSpace(reward.Description))
                {
                    description.Amount = "";
                    description.Tooltip = reward.Description;
                }
            }

            return descriptions;
        }

        private static RewardDescription[] GetRawDescriptions(CompositeRewardObject reward,
            DescriptionVisibility visibility, Guid goldResourceId, double? rewardFactor = null)
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
                            Tooltip = $"Secret reward",
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
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = string.Empty,
                            IconUrl = x.IconUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = MaskResourcesAmount(x, reward.Amounts[i], goldResourceId),
                            IconUrl = x.IconUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ApproximateResourcesAmount(x, reward.Amounts[i], goldResourceId),
                            IconUrl = x.IconUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.Resources.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = reward.Amounts[i].ToString(),
                            IconUrl = x.IconUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
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
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.UnitTypes.Select(x => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = string.Empty,
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ArmySizeDescription.MaskArmySize(reward.Amounts[i]),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = ArmySizeDescription.ApproximateArmySize(reward.Amounts[i]),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = reward.Amounts[i].ToString(),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.UnitsToBuy)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "$x?",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Buy {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = FormatUnitsToBuyAmount(reward.Amounts, i, x.ToTrainAmount, rewardFactor),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Buy {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = FormatUnitsToBuyAmount(reward.Amounts, i, x.ToTrainAmount, rewardFactor),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Buy {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = FormatUnitsToBuyAmount(reward.Amounts, i, x.ToTrainAmount, rewardFactor),
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Buy {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }
            
            if (reward.RewardType == RewardType.UnitsToUpgrade)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "^",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Upgrade to {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.MaskedSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "^",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Upgrade to {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.ApproximateSize:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "^",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Upgrade to {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    case DescriptionVisibility.Full:
                        return reward.UnitTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = "^",
                            IconUrl = x.BattleImgUrl ?? string.Empty,
                            Tooltip = $"Upgrade to {x.Name} for {DescribeResourcesAmounts(reward.Prices[i])}"
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.ArtifactsGain)
            {
                int SafeAmount(int index, int defaultValue = 1)
                {
                    if (reward.Amounts == null || index < 0 || index >= reward.Amounts.Length)
                        return defaultValue;
                    return reward.Amounts[index];
                }

                string FormatAmount(int amount) => amount > 1 ? $"x{amount}" : string.Empty;

                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new[] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return reward.ArtifactTypes.Select((x, i) => new RewardDescription
                        {
                            Name = x.Name,
                            Amount = FormatAmount(SafeAmount(i)),
                            IconUrl = x.ThumbnailUrl ?? string.Empty,
                            Tooltip = $"Gain {x.Name}",
                        }).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.Attack)
            {
                var amount = reward.Amounts.Length > 0 ? reward.Amounts[0] : 0;
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                        return new [] { new RewardDescription
                        {
                            Name = "Attack",
                            Amount = "?",
                            IconUrl = string.Empty,
                            Tooltip = $"Gain Attack",
                        } };
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = "Attack",
                            Amount = amount > 0 ? $"+{amount}" : string.Empty,
                            IconUrl = string.Empty,
                            Tooltip = $"Gain {amount} Attack",
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.Defense)
            {
                var amount = reward.Amounts.Length > 0 ? reward.Amounts[0] : 0;
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new [] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = $"Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                        return new [] { new RewardDescription
                        {
                            Name = "Defense",
                            Amount = "?",
                            IconUrl = string.Empty,
                            Tooltip = $"Gain Defense",
                        } };
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = "Defense",
                            Amount = amount > 0 ? $"+{amount}" : string.Empty,
                            IconUrl = string.Empty,
                            Tooltip = $"Gain {amount} Defense",
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.NextStage)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new [] { new RewardDescription
                        {
                            Name = "Next Stage",
                            Amount = string.Empty,
                            IconUrl = string.Empty,
                            Tooltip = "Advance to the next stage",
                            IsNextStage = true,
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            if (reward.RewardType == RewardType.Magic)
            {
                switch (visibility)
                {
                    case DescriptionVisibility.None:
                        return new[] { new RewardDescription
                        {
                            Name = "Unknown",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.QuestionIconUrl,
                            Tooltip = "Secret reward",
                        } };
                    case DescriptionVisibility.TypeOnly:
                    case DescriptionVisibility.MaskedSize:
                    case DescriptionVisibility.ApproximateSize:
                    case DescriptionVisibility.Full:
                        return new[] { new RewardDescription
                        {
                            Name = reward.Title ?? "Magic",
                            Amount = string.Empty,
                            IconUrl = PredefinedStaticResources.MagicScrollIconUrl,
                            Tooltip = reward.Description ?? "Learn a new spell",
                        } };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null);
                }
            }

            throw new NotImplementedException();
        }

        private static string DescribeResourcesAmounts(ResourceAmount[] resourceAmounts)
        {
            return string.Join(", ", resourceAmounts.Select(x => x.Name));
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
            if (amount <= 7)    return "Bundle";
            if (amount <= 14)   return "Satchel";
            if (amount <= 24)   return "Pouch";
            if (amount <= 39)   return "Box";
            if (amount <= 59)   return "Chest";
            if (amount <= 98)   return "Vault";
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

        private static string FormatUnitsToBuyAmount(int[] amounts, int index, int toTrainAmount, double? rewardFactor)
        {
            if (!rewardFactor.HasValue || amounts == null || index < 0 || index >= amounts.Length)
                return "$";
            
            var targetAmount = amounts[index];
            if (toTrainAmount <= 0 || rewardFactor.Value <= 0)
                return "$";
            
            var baseAmount = (int)(toTrainAmount * rewardFactor.Value);
            if (baseAmount <= 0)
                return "$";
            
            // Calculate how many times the target amount is greater than the base amount
            var multiplier = (double)targetAmount / baseAmount;
            
            // Only add postfix if multiplier is at least 2 (meaning at least 2x the base)
            if (multiplier >= 2.0)
            {
                var n = (int)multiplier;
                return $"$x{n}";
            }
            
            return "$";
        }
    }
}