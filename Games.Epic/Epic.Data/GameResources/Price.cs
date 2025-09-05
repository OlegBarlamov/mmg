using System;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Data.GameResources
{
    public class Price
    {
        public Dictionary<Guid, int> PerResourcePrice { get; private set; } = new Dictionary<Guid, int>();

        private Price() { }

        public void MultiplyBy(int multiplier)
        {
            PerResourcePrice.Keys.ToList().ForEach(key =>
            {
                PerResourcePrice[key] *= multiplier;
            });
        }

        public void Invert()
        {
            PerResourcePrice.Keys.ToList().ForEach(key =>
            {
                PerResourcePrice[key] = -PerResourcePrice[key];
            });
        }
        
        public static Price Empty()
        {
            return new Price();
        }

        public bool IsEmpty()
        {
            return PerResourcePrice.Count == 0;
        }

        public static Price Combine(IEnumerable<Price> prices)
        {
            var result = new Price();
            foreach (var price in prices)
            {
                price.PerResourcePrice.Keys.ToList().ForEach(key =>
                {
                    var value = price.PerResourcePrice[key];
                    if (!result.PerResourcePrice.TryAdd(key, value))
                        result.PerResourcePrice[key] += value;
                });
            }
            return result;
        }
        
        public static Price Create(IReadOnlyDictionary<Guid, int> perResourcePrice)
        {
            return new Price
            {
                PerResourcePrice = new Dictionary<Guid, int>(perResourcePrice)
            };
        }
    }
}