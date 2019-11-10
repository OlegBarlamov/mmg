using System.Collections.Generic;

namespace Gates.Core.ServerApi
{
    public sealed class RoomState
    {
        public string Name { get; set; }

        public string Owner { get; set; }

        public IReadOnlyList<string> Users { get; set; }

        public bool GameStarted { get; set; }
    }
}
