using Epic.Data.GameResources;

namespace Epic.Server.Resources
{
    public class ResourceDashboardResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        
        public ResourceDashboardResource(ResourceAmount resourceAmount)
        {
            Id = resourceAmount.ResourceId.ToString();
            Name = resourceAmount.Name;
            IconUrl = resourceAmount.IconUrl;
            Amount = resourceAmount.Amount;
            Price = resourceAmount.Price;
        }
    }
}