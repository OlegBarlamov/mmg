using Gates.Core.Models;

namespace Gates.GameLogic
{
    internal static class MachineDataExtensions
    {
        public static int GetDirectionMultiplier(this MachineData machine)
        {
            if (machine.Direction)
                return -1;

            return 1;
        }
    }
}
