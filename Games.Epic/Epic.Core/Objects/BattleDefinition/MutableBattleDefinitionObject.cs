using System;
using System.Collections.Generic;
using Epic.Core.Objects;

namespace Epic.Core
{
    public class MutableBattleDefinitionObject : IBattleDefinitionObject
    {
        public Guid Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IReadOnlyCollection<Guid> UnitsIds { get; set; }
        public IReadOnlyCollection<IUserUnitObject> Units { get; set; }
    }
}