using System;
using System.Drawing;
using Gates.Core.Models;

namespace Gates.GameLogic
{
    internal class MachineProcessor : IGameProcessor
    {
        private MachineData Machine { get; }

        public MachineProcessor(MachineData machine)
        {
            Machine = machine ?? throw new ArgumentNullException(nameof(machine));
        }

        public void Process(int elapsedTimeMs)
        {
            var speed = (float)((100 + Machine.AdditionalSpeed) * Machine.GetDirectionMultiplier());
            var positionDelta = speed * elapsedTimeMs / 1000;
            if (positionDelta > float.Epsilon)
            {
                var positionDeltaInteger = positionDelta < 1 ? 1 : (int)positionDelta;
                Machine.Position = new Point(Machine.Position.X + positionDeltaInteger, Machine.Position.Y);
            }
        }
    }
}
