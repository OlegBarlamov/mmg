using Epic.Core.Services.Heroes;

namespace Epic.Core.Logic
{
    public interface IExperienceGainResult
    {
        public int ExperienceGain { get; }
        public int LevelsGain { get; }
        public int AttacksGain { get; }
        public int DefenseGain { get; }
        public int KnowledgeGain { get; }
        public int PowerGain { get; }
        public int ArmySlotsGain { get; }
    }
    
    public interface IHeroesLevelsCalculator
    {
        IExperienceGainResult GiveExperience(IHeroObject heroObject, int experienceGain);
    }
}