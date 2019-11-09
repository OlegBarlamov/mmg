using System.Collections.Generic;
using Gates.Core.Models;

namespace Gates.GameLogic
{
    internal class SceneProcessor : IGameProcessor
    {
        private IReadOnlyList<MachineData> Machines { get; }

        public SceneProcessor(IReadOnlyList<MachineData> sceneMachines)
        {
            Machines = sceneMachines;
        }

        public void Process(int elapsedTimeMs)
        {
            foreach (var machine in Machines)
            {
                var processor = new MachineProcessor(machine);
                processor.Process(elapsedTimeMs);
            }
        }
    }
}