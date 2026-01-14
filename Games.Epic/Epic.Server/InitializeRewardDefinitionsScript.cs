using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.RewardDefinitions;
using Epic.Core.Services.UnitTypes;
using Epic.Data.Reward;
using Epic.Data.RewardDefinitions;
using FrameworkSDK;
using JetBrains.Annotations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Epic.Server;

internal class RewardDefinitionsConfig
{
    [UsedImplicitly]
    internal class RewardDefinitionGuardConfigDeclaration
    {
        public string Message { get; set; }
        public string[] Units { get; set; }
        public int[] AmountsMin { get; set; }
        public int[] AmountsMax { get; set; }
        public int[] Amounts { get; set; }
        public int BattleMinWidth { get; set; }
        public int BattleMaxWidth { get; set; }
        public int BattleMinHeight { get; set; }
        public int BattleMaxHeight { get; set; }
        public int BattleWidth { get; set; }
        public int BattleHeight { get; set; }
    }
    
    [UsedImplicitly]
    internal class RewardDefinitionConfigDeclaration
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public RewardType RewardType { get; set; }
        public string[] Units { get; set; }
        public string[] Resources { get; set; }
        public int[] AmountsMin { get; set; }
        public int[] AmountsMax { get; set; }
        public int[] Amounts { get; set; }
        public string IconUrl { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public bool CanDecline { get; set; } = true;
        
        public RewardDefinitionGuardConfigDeclaration Guard { get; set; }
    }
    
    [UsedImplicitly]
    public Dictionary<string, RewardDefinitionConfigDeclaration> Rewards { get; set; }
}

[UsedImplicitly]
public class InitializeRewardDefinitionsScript : IAppComponent
{
    public IRewardDefinitionsRepository Repository { get; }
    public IUnitTypesRegistry UnitTypesRegistry { get; }
    public DefaultRewardDefinitionsRegistry RewardDefinitionsRegistry { get; }
    public IGameResourcesRegistry ResourcesRegistry { get; }

    public InitializeRewardDefinitionsScript(
        [NotNull] IRewardDefinitionsRepository repository,
        [NotNull] IUnitTypesRegistry unitTypesRegistry,
        [NotNull] DefaultRewardDefinitionsRegistry rewardDefinitionsRegistry,
        [NotNull] IGameResourcesRegistry resourcesRegistry)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        UnitTypesRegistry = unitTypesRegistry ?? throw new ArgumentNullException(nameof(unitTypesRegistry));
        RewardDefinitionsRegistry = rewardDefinitionsRegistry ?? throw new ArgumentNullException(nameof(rewardDefinitionsRegistry));
        ResourcesRegistry = resourcesRegistry ?? throw new ArgumentNullException(nameof(resourcesRegistry));
    }
    
    public void Configure()
    {
        ProcessAsync().Wait();
    }

    private async Task ProcessAsync()
    {
        try
        {
            var config = YamlConfigParser<RewardDefinitionsConfig>
                .Parse("Configs/rewards.yaml");

            var rewardDefinitionFields = config.Rewards.Select(keyValue =>
            {
                var x = keyValue.Value;
                return new RewardDefinitionFields
                {
                    Key = x.Key,
                    RewardType = x.RewardType,
                    Value = x.Value,
                    Name = x.Key,
                    Title = x.Title,
                    IconUrl = x.IconUrl,
                    Message = x.Message,
                    MaxAmounts = x.Amounts ?? x.AmountsMax ?? Array.Empty<int>(),
                    MinAmounts = x.Amounts ?? x.AmountsMin ?? Array.Empty<int>(),
                    CanDecline = x.CanDecline,
                    Ids = x.Units?.Select(GetUnitIdByName).ToArray() 
                          ?? x.Resources?.Select(GetResourceIdByName).ToArray()
                          ?? Array.Empty<Guid>(),
                    GuardMessage = x.Guard?.Message,
                    GuardUnitTypeIds = x.Guard?.Units.Select(GetUnitIdByName).ToArray() 
                                       ?? Array.Empty<Guid>(),
                    GuardUnitMinAmounts = x.Guard?.Amounts ?? x.Guard?.AmountsMin,
                    GuardUnitMaxAmounts = x.Guard?.Amounts ?? x.Guard?.AmountsMax,
                    GuardBattleMinWidth = x.Guard?.BattleWidth ?? x.Guard?.BattleMinWidth ?? 0,
                    GuardBattleMaxWidth = x.Guard?.BattleHeight ?? x.Guard?.BattleMaxWidth ?? 0,
                    GuardBattleMaxHeight = x.Guard?.BattleHeight ?? x.Guard?.BattleMaxHeight ?? 0,
                    GuardBattleMinHeight = x.Guard?.BattleMinHeight ?? x.Guard?.BattleMinHeight ?? 0,
                };
            });
        
            await Repository.CreateBatch(rewardDefinitionFields);

            await RewardDefinitionsRegistry.Load();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
        }
    }

    private Guid GetUnitIdByName(string unitTypeName)
    {
        return UnitTypesRegistry.ByKey(unitTypeName).Id;
    }

    private Guid GetResourceIdByName(string resourceName)
    {
        return ResourcesRegistry.GetByKey(resourceName).Id;
    }
    
    public void Dispose()
    {
        
    }
}