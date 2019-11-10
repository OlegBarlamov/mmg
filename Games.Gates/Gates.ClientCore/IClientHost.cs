namespace Gates.ClientCore
{
    public interface IClientHost
    {
        void ConnectToServer(string serverUrl);

        void Authorize(string name);

        void CreateRoom(string name, string password);

        void EnterRoom(string name, string password);

        void RunGame();
    }
}
