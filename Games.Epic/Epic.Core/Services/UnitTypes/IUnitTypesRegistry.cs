using System;
using System.Collections.Generic;
using Epic.Data.UnitTypes;

namespace Epic.Core.Services.UnitTypes
{
    public interface IUnitTypesRegistry
    {
        IReadOnlyList<IUnitTypeEntity> AllOrderedByValue { get; }
        IReadOnlyList<IUnitTypeEntity> ToTrainOrderedByValue { get; }
        
        IUnitTypeEntity ById(Guid typeId);

        IReadOnlyList<IUnitTypeEntity> FindUpgradesFor(Guid typeId);
    }
}