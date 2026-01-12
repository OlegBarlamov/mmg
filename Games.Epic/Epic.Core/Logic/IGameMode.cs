namespace Epic.Core.Logic
{
    public interface IGameMode
    {
        bool SynchronizedPlayers { get; }
        IGameModeStage[] Stages { get; }
    }

    public interface IGameModeStage
    {
        double RewardsFactor { get; }
        double DifficultyFactor { get; }
        double MinDifficultyFactor { get; }
        int StartMinDifficulty { get; }
        double MaxDifficultyFactor { get; }
        int StartMaxDifficulty { get; }
        bool PvpBattles { get; }
        int BattlesLimit { get; }
        int DaysLimit { get; }
        int WideStdChance { get; }
        double BattlesCountFactor { get; }
    }
}