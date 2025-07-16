using System;
using Epic.Core;
using Epic.Data.UnitTypes;

namespace Epic.Server.Resources
{
    public class UnitRewardResource
    {
        public Guid Id { get; }
        public string Name { get; }
        public string DashboardImgUrl { get; }

        public IUnitProps Props { get; }
        
        public int Amount { get; }
        
        public UnitRewardResource(IUnitTypeObject unitTypeObject, int amount)
        {
            Id = unitTypeObject.Id;
            Name = unitTypeObject.Name;
            DashboardImgUrl = unitTypeObject.DashboardImgUrl;
            Props = unitTypeObject;
            Amount = amount;
        }
    }
}