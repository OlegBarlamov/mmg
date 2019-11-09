using System.Collections.Generic;

namespace Gates.Core.Models
{
    public class PlayerGameData
    {
        public int GoldCount { get; set; }

        public int Lifes { get; set; }

        public MachineData[] Slots { get; set; }

        public List<Particle> ParticlesQueue { get; set; }

        public List<ProgressContainer<Particle>> ConstructingParticles { get; set; }
    }
}
