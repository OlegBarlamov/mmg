using Gates.Core.Models;

namespace Gates.Core.ServerApi
{
    public interface IServerGatesApi
    {
        void UserReady();

        GameStartInfo StartInfo();

        GameDataIteration GemeState();
    }
}
