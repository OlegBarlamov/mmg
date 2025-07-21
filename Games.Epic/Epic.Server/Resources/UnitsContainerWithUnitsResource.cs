using System.Linq;
using Epic.Core.Services.UnitsContainers;

namespace Epic.Server.Resources
{
    public class UnitsContainerWithUnitsResource
    {
        public string Id { get; }
        public int Capacity { get; }
        public UserUnitInDashboardResource[] Units { get; }
        
        public UnitsContainerWithUnitsResource(IUnitsContainerWithUnits containerWithUnits)
        {
            Id = containerWithUnits.Id.ToString();
            Capacity = containerWithUnits.Capacity;
            Units = containerWithUnits.Units.Select(x => new UserUnitInDashboardResource(x)).ToArray();
        }
    }
}