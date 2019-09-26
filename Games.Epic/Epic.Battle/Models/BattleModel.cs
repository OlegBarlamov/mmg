using System;
using Epic.Data.Battle;
using JetBrains.Annotations;
using NetExtensions;

namespace Epic.Battle.Models
{
    public class BattleModel
    {
        public BattleField Field { get; }

        public BattleModel([NotNull] BattleField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public static BattleModel Generate(Int32Size size)
        {
            var field = new BattleField(size);
            return new BattleModel(field);
        }
    }
}