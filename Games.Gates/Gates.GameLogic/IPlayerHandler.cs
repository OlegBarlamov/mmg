using System.Drawing;

namespace Gates.GameLogic
{
    public interface IPlayerHandler
    {
        bool ProduceMachineFromSlot(int playerID, int slotID);

        bool ClearSlot(int playerID, int slotID);


        bool StartConstruct(int playerID, int particleID);

        bool ProduceParticleToSlot(int playerID, int slotID, Point position);

        bool QueueScroll(int playerID);
    }
}
