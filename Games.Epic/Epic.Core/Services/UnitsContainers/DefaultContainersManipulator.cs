using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers.Errors;
using Epic.Data.GlobalUnits;
using Epic.Data.PlayerUnits;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Core.Services.UnitsContainers
{
    [UsedImplicitly]
    public class DefaultContainersManipulator : IContainersManipulator
    {
        private IGlobalUnitsRepository GlobalUnitsRepository { get; }
        private IUnitsContainersService UnitsContainersService { get; }
        private IGlobalUnitsService GlobalUnitsService { get; }

        public DefaultContainersManipulator(
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IGlobalUnitsService globalUnitsService)
        {
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
        }

        public async Task<IUnitsContainerWithUnits> GetContainerWithUnits(Guid id)
        {
            var container = await UnitsContainersService.GetById(id);
            var units = await GlobalUnitsService.GetAliveUnitsByContainerId(id);
            return new UnitsContainerWithUnits(container, units.ToArray());
        }

        public async Task<IUnitsContainerWithUnits> PlaceUnitsToContainer(Guid containerId, params IGlobalUnitObject[] units)
        {
            if (containerId == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();
            
            var container = await UnitsContainersService.GetById(containerId);
            var emptySlots = await FindEmptySlots(container);
            if (emptySlots.Length < units.Length)
                throw new EmptySlotNotFoundException(container);
            
            var unitsEntities = units.Select(x => x.ToEntity()).ToArray();
            for (int i = 0; i < unitsEntities.Length; i++)
            {
                var entity = unitsEntities[i];
                entity.ContainerId = containerId;
                entity.ContainerSlotIndex = emptySlots[i];
            }

            await GlobalUnitsRepository.Update(unitsEntities);
            
            return await GetContainerWithUnits(containerId);
        }

        public async Task<IUnitsContainerWithUnits> MoveUnits(IGlobalUnitObject originalUnit, Guid targetContainerId, int? amountToSplitOptional,
            int? specificSlotIndex = null)
        {
            if (targetContainerId == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();
            
            var amountToSplit = amountToSplitOptional ?? originalUnit.Count;
            if (originalUnit.Count < amountToSplit)
                throw new AmountOfUnitsInSlotExceededException(originalUnit, amountToSplit);

            var moveAllUnits = amountToSplit == originalUnit.Count;
            var container = await UnitsContainersService.GetById(targetContainerId);
            var emptySlots = await FindEmptySlots(container);
            if (!specificSlotIndex.HasValue && emptySlots.Length == 0)
                throw new EmptySlotNotFoundException(container);

            var originalUnitEntity = originalUnit.ToEntity();
            if (specificSlotIndex.HasValue && !emptySlots.Contains(specificSlotIndex.Value))
            {
                var targetUnit = await GlobalUnitsService.GetAliveUnitFromContainerInSlot(targetContainerId, specificSlotIndex.Value);
                if (originalUnit.Id == targetUnit.Id)
                    throw new InvalidUnitSlotsOperationException("Target slot contains exact same unit object.");
                    
                if (originalUnit.UnitType.Id != targetUnit.UnitType.Id)
                {
                    if (!moveAllUnits)
                        throw new SlotIsOccupiedException(container, specificSlotIndex.Value);
                    
                    await ExchangeUnitSlots(originalUnit, targetUnit);
                    return await GetContainerWithUnits(targetContainerId);
                }
                
                // Combine units same type

                var targetUnitEntity = targetUnit.ToEntity();

                targetUnitEntity.Count += amountToSplit;
                originalUnitEntity.Count -= amountToSplit;
                if (originalUnitEntity.Count < 1)
                {
                    originalUnitEntity.ContainerId = DefaultContainers.EmptyContainerId;
                }
                
                await GlobalUnitsRepository.Update(originalUnitEntity, targetUnitEntity);
                return await GetContainerWithUnits(targetContainerId);
            }
            
            // Move to an Empty slot
            
            var targetSlot = specificSlotIndex ?? emptySlots.First();
            if (moveAllUnits)
            {
                originalUnitEntity.ContainerSlotIndex = targetSlot;
                originalUnitEntity.ContainerId = container.Id;
            }
            else
            {
                await GlobalUnitsRepository.Create(originalUnit.UnitType.Id, amountToSplit,
                    targetContainerId, originalUnit.IsAlive, targetSlot);
                
                originalUnitEntity.Count -= amountToSplit;
            }
            
            await GlobalUnitsRepository.Update(originalUnitEntity);

            return await GetContainerWithUnits(targetContainerId);
        }

        public async Task<IUnitsContainerWithUnits> FillEmptySlotsWithUnits(IGlobalUnitObject originalUnit, Guid targetContainerId)
        {
            if (targetContainerId == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();
                    
            var container = await UnitsContainersService.GetById(targetContainerId);
            var emptySlots = await FindEmptySlots(container);
            
            if (originalUnit.Count < emptySlots.Length + 1) 
                throw new AmountOfUnitsInSlotExceededException(originalUnit, emptySlots.Length + 1);
            
            if (emptySlots.Length != 0)
            {
                var originalUnitEntity = originalUnit.ToEntity();

                await Task.WhenAll(emptySlots.Select(i => GlobalUnitsRepository.Create(originalUnit.UnitType.Id, 1, targetContainerId, originalUnit.IsAlive, i)));
            
                originalUnitEntity.Count -= emptySlots.Length;
                await GlobalUnitsRepository.Update(originalUnitEntity);
            }

            return await GetContainerWithUnits(targetContainerId);
        }

        public async Task ExchangeContainers(Guid container1Id, Guid container2Id)
        {
            if (container1Id == DefaultContainers.EmptyContainerId || container2Id == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();

            var container1 = await UnitsContainersService.GetById(container1Id);
            var units1 = await GlobalUnitsRepository.GetAliveByContainerId(container1Id);
            var container2 = await UnitsContainersService.GetById(container2Id);
            var units2 = await GlobalUnitsRepository.GetAliveByContainerId(container2Id);
            
            var units1MaxSlotIndex = units1.Max(u => u.ContainerSlotIndex);
            var units2MaxSlotIndex = units2.Max(u => u.ContainerSlotIndex);
            
            if (units1MaxSlotIndex >= container2.Capacity || units2MaxSlotIndex >= container1.Capacity)
                throw new InvalidUnitSlotsOperationException("Capacity of slots are not enough for exchanging units.");
            
            units1.ForEach(u =>
            {
                u.ContainerId = container2.Id;
            });
            units2.ForEach(u =>
            {
                u.ContainerId = container1.Id;
            });
            
            await GlobalUnitsRepository.Update(units1.Union(units2).ToArray());
        }

        public async Task<Pair<IGlobalUnitObject>> ExchangeUnitSlots(IGlobalUnitObject unit1, IGlobalUnitObject unit2)
        {
            var unit1Entity = unit1.ToEntity();
            var unit2Entity = unit2.ToEntity();
            
            unit1Entity.ContainerId = unit2.ContainerId;
            unit1Entity.ContainerSlotIndex = unit2.ContainerSlotIndex;
            unit2Entity.ContainerId = unit1.ContainerId;
            unit2Entity.ContainerSlotIndex = unit1.ContainerSlotIndex;

            await GlobalUnitsRepository.Update(unit1Entity, unit2Entity);

            return new Pair<IGlobalUnitObject>(
                MutableGlobalUnitObject.FromEntity(unit1Entity),
                MutableGlobalUnitObject.FromEntity(unit2Entity)
            );
        }

        public async Task<Pair<IGlobalUnitObject>> ExchangeUnitsSameType(IGlobalUnitObject fromUnit, IGlobalUnitObject toUnit, int amountToGive)
        {
            if (fromUnit.Count < amountToGive)
                throw new AmountOfUnitsInSlotExceededException(fromUnit, amountToGive);
            if (fromUnit.UnitType.Id != toUnit.UnitType.Id)
                throw new InvalidUnitSlotsOperationException("Units are not the same type for exchanging.");
            
            var fromUnitEntity = fromUnit.ToEntity();
            var toUnitEntity = toUnit.ToEntity();
            
            fromUnitEntity.Count -= amountToGive;
            toUnitEntity.Count += amountToGive;

            if (fromUnitEntity.Count < 1)
            {
                fromUnitEntity.ContainerId = DefaultContainers.EmptyContainerId;
            }

            await GlobalUnitsRepository.Update(fromUnitEntity, toUnitEntity);
            
            return new Pair<IGlobalUnitObject>(
                MutableGlobalUnitObject.FromEntity(fromUnitEntity),
                MutableGlobalUnitObject.FromEntity(toUnitEntity)
            );
        }

        private async Task<int[]> FindEmptySlots(IUnitsContainerObject container)
        {
            var existingUnitsInContainer = await GlobalUnitsRepository.GetAliveByContainerId(container.Id);
            return Enumerable.Range(0, container.Capacity)
                .Where(i => existingUnitsInContainer
                    .All(u => u.ContainerSlotIndex != i)
                ).ToArray();
        }
    }
}