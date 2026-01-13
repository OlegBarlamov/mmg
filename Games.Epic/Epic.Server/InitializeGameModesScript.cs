using System;
using System.Linq;
using Epic.Core.Logic;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server;

internal class GameModesConfigDeclaration
{
    internal class GameModeDeclaration : IGameMode
    {
        public string Name { get; set; }

        public bool SynchronizedPlayers { get; set; } = false;
        public int InitialArmyScore { get; set; } = 1200;
        public GameModeStageDeclaration[] Stages { get; set; } = Array.Empty<GameModeStageDeclaration>();
        
        IGameModeStage[] IGameMode.Stages => Stages;
    }

    internal class GameModeStageDeclaration : IGameModeStage
    {
        public double RewardsFactor { get; set; } = 1;
        public double DifficultyFactor { get; set; } = 1;
        
        public double MinDifficultyFactor { get; set; }
        public int StartMinDifficulty { get; set; }
        public double MaxDifficultyFactor { get; set; }
        public int StartMaxDifficulty { get; set; }
        
        public bool PvpBattles { get; set; } = false;
        public int BattlesLimit { get; set; } = 0;
        public int DaysLimit { get; set; } = 0;
        public int WideStdChance { get; set; } = 0;
        public double BattlesCountFactor { get; set; } = 1;
    }
    
    public string Active { get; set; }
    public GameModeDeclaration[] Modes { get; set; } = Array.Empty<GameModeDeclaration>();
}

[UsedImplicitly]
internal class InitializeGameModesScript : IAppComponent
{
    public FixedGameModeProvider GameModeProvider { get; }

    public InitializeGameModesScript([NotNull] FixedGameModeProvider gameModeProvider)
    {
        GameModeProvider = gameModeProvider ?? throw new ArgumentNullException(nameof(gameModeProvider));
    }
    
    public void Dispose()
    {
    }

    public void Configure()
    {
        var config = YamlConfigParser<GameModesConfigDeclaration>
            .Parse("Configs/modes.yaml");
        
        var activeDeclaration = config.Modes.First(x => x.Name == config.Active);
        
        GameModeProvider.SetGameMode(activeDeclaration);
    }
    
}