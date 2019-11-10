namespace Gates.Core.ServerApi
{
    public interface IServer
    {
        IServerApi Authorize(string name, string password);
    }
}
