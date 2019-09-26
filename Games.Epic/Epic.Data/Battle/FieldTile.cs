using System;
using JetBrains.Annotations;

namespace Epic.Data.Battle
{
    public class FieldTile
    {
        public static FieldTile Default { get; } = new FieldTile(0, "empty");

        public int ID { get; }

        public string Name { get; }

        public FieldTile(int id, [NotNull] string name)
        {
            ID = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
