namespace Gates.Core.ServerApi
{
    public interface IServerRoomApi
    {
        void LeaveRoom();

        RoomState GetState();

        IServerGatesApi RunGatesGame();
    }
}
