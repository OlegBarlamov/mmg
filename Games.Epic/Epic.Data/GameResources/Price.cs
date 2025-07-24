using System;
using System.Collections.Generic;

namespace Epic.Data.GameResources
{
    public class Price
    {
        public Dictionary<Guid, int> PerResourcePrice { get; private set; } = new Dictionary<Guid, int>();

        public static Price Create(IReadOnlyDictionary<Guid, int> perResourcePrice)
        {
            return new Price
            {
                PerResourcePrice = new Dictionary<Guid, int>(perResourcePrice)
            };
        }
    }
}