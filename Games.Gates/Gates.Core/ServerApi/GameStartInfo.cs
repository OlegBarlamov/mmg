using System.Collections.Generic;

namespace Gates.Core.ServerApi
{
    public sealed class GameStartInfo
    {
        public IReadOnlyList<UserReadyInfo> UsersInfo { get; set; }

        public bool Runned { get; set; }

        public int BeforeStartMs { get; set; }
    }
}
