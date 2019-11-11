namespace Gates.Core.ServerApi
{
    public interface IServerRoomApi
    {
        void LeaveRoom();

        RoomState GetState();

        void RunGatesGame();

        IServerGatesApi ConnectToRunnedGame();
    }
}
