using System.Linq;
using Epic.Data.GameResources;

namespace Epic.Server.Resources
{
    public class PriceResource
    {
        public ResourceDashboardResource[] Resources { get; set; }

        public PriceResource(ResourceAmount[] resourceAmount)
        {
            Resources = resourceAmount.Select(x => new ResourceDashboardResource(x)).ToArray();
        }
    }
}