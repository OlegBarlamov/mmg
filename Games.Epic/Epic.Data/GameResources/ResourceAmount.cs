using System;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Data.GameResources
{
    public class ResourceAmount
    {
        public Guid ResourceId { get; private set; }
        public string Name { get; private set; }
        public string IconUrl { get; private set; }
        public int Price { get; private set; }
        public int Amount { get; private set; }

        private ResourceAmount() { }

        public static ResourceAmount Create(
            IGameResourceEntity resource,
            int amount)
        {
            return new ResourceAmount
            {
                ResourceId = resource.Id,
                Name = resource.Name,
                IconUrl = resource.IconUrl,
                Price = resource.Price,
                Amount = amount,
            };
        }
    }
    
    public static class ResourceAmountExtensions 
    {
        public static Price ToPrice(this IEnumerable<ResourceAmount> resourceAmount, bool invertPrice = false)
        {
            var dictionary = resourceAmount.ToDictionary(x => x.ResourceId, x => invertPrice ? -x.Amount : x.Amount);
            return Price.Create(dictionary);
        }
    }
}
