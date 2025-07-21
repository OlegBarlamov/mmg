using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers.Errors;
using Epic.Data.PlayerUnits;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Core.Services.UnitsContainers
{
    [UsedImplicitly]
    public class DefaultContainersManipulator : IContainersManipulator
    {
        private IPlayerUnitsRepository PlayerUnitsRepository { get; }
        private IUnitsContainersService UnitsContainersService { get; }
        private IPlayerUnitsService PlayerUnitsService { get; }

        public DefaultContainersManipulator(
            [NotNull] IPlayerUnitsRepository playerUnitsRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IPlayerUnitsService playerUnitsService)
        {
            PlayerUnitsRepository = playerUnitsRepository ?? throw new ArgumentNullException(nameof(playerUnitsRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
        }

        public async Task<IUnitsContainerWithUnits> GetContainerWithUnits(Guid id)
        {
            var container = await UnitsContainersService.GetById(id);
            var units = await PlayerUnitsService.GetAliveUnitsByContainerId(id);
            return new UnitsContainerWithUnits(container, units.ToArray());
        }

        public async Task<IUnitsContainerWithUnits> PlaceUnitsToContainer(Guid containerId, params IPlayerUnitObject[] units)
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
                entity.PlayerId = container.OwnerPlayerId;
            }

            await PlayerUnitsRepository.Update(unitsEntities);
            
            return await GetContainerWithUnits(containerId);
        }

        public async Task<IUnitsContainerWithUnits> SplitUnits(IPlayerUnitObject originalUnit, Guid targetContainerId, int amountToSplit,
            int? specificSlotIndex = null)
        {
            if (targetContainerId == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();
            if (originalUnit.Count < amountToSplit)
                throw new AmountOfUnitsInSlotExceededException(originalUnit, amountToSplit);
            
            var container = await UnitsContainersService.GetById(originalUnit.ContainerId);
            var emptySlots = await FindEmptySlots(container);
            if (emptySlots.Length == 0)
                throw new EmptySlotNotFoundException(container);
            if (specificSlotIndex.HasValue && !emptySlots.Contains(specificSlotIndex.Value))
                throw new SlotIsOccupiedException(container, specificSlotIndex.Value);
            
            var targetSlot = specificSlotIndex ?? emptySlots.First();

            await PlayerUnitsRepository.CreatePlayerUnit(originalUnit.UnitType.Id, amountToSplit, container.OwnerPlayerId,
                targetContainerId, originalUnit.IsAlive, targetSlot);

            var originalUnitEntity = originalUnit.ToEntity();
            originalUnitEntity.Count -= amountToSplit;
            
            await PlayerUnitsRepository.Update(originalUnitEntity);

            return await GetContainerWithUnits(targetContainerId);
        }

        public async Task<IUnitsContainerWithUnits> FillEmptySlotsWithUnits(IPlayerUnitObject originalUnit, Guid targetContainerId)
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

                await Task.WhenAll(emptySlots.Select(i => PlayerUnitsRepository.CreatePlayerUnit(originalUnit.UnitType.Id, 1,
                    container.OwnerPlayerId, targetContainerId, originalUnit.IsAlive, i)));
            
                originalUnitEntity.Count -= emptySlots.Length;
                await PlayerUnitsRepository.Update(originalUnitEntity);
            }

            return await GetContainerWithUnits(targetContainerId);
        }

        public async Task ExchangeContainers(Guid container1Id, Guid container2Id)
        {
            if (container1Id == DefaultContainers.EmptyContainerId || container2Id == DefaultContainers.EmptyContainerId)
                throw new TargetContainerIdIsEmptyContainerException();

            var container1 = await UnitsContainersService.GetById(container1Id);
            var units1 = await PlayerUnitsRepository.GetAliveByContainerId(container1Id);
            var container2 = await UnitsContainersService.GetById(container2Id);
            var units2 = await PlayerUnitsRepository.GetAliveByContainerId(container2Id);
            
            var units1MaxSlotIndex = units1.Max(u => u.ContainerSlotIndex);
            var units2MaxSlotIndex = units2.Max(u => u.ContainerSlotIndex);
            
            if (units1MaxSlotIndex >= container2.Capacity || units2MaxSlotIndex >= container1.Capacity)
                throw new InvalidUnitSlotsOperationException("Capacity of slots are not enough for exchanging units.");
            
            units1.ForEach(u =>
            {
                u.ContainerId = container2.Id;
                u.PlayerId = container2.OwnerPlayerId;
            });
            units2.ForEach(u =>
            {
                u.ContainerId = container1.Id;
                u.PlayerId = container1.OwnerPlayerId;
            });
            
            await PlayerUnitsRepository.Update(units1.Union(units2).ToArray());
        }

        public async Task<Pair<IPlayerUnitObject>> ExchangeUnitSlots(IPlayerUnitObject unit1, IPlayerUnitObject unit2)
        {
            var unit1Entity = unit1.ToEntity();
            var unit2Entity = unit2.ToEntity();

            unit1Entity.ContainerId = unit2Entity.ContainerId;
            unit1Entity.PlayerId = unit2Entity.PlayerId;
            unit1Entity.ContainerSlotIndex = unit2Entity.ContainerSlotIndex;
            unit2Entity.ContainerId = unit1Entity.ContainerId;
            unit2Entity.ContainerSlotIndex = unit1Entity.ContainerSlotIndex;
            unit2Entity.PlayerId = unit1Entity.PlayerId;

            await PlayerUnitsRepository.Update(unit1Entity, unit2Entity);

            return new Pair<IPlayerUnitObject>(
                MutablePlayerUnitObject.FromEntity(unit1Entity),
                MutablePlayerUnitObject.FromEntity(unit2Entity)
            );
        }

        public async Task<Pair<IPlayerUnitObject>> ExchangeUnitsSameType(IPlayerUnitObject fromUnit, IPlayerUnitObject toUnit, int amountToGive)
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
                fromUnitEntity.ContainerId = Guid.Empty;
            
            await PlayerUnitsRepository.Update(fromUnitEntity, toUnitEntity);
            
            return new Pair<IPlayerUnitObject>(
                MutablePlayerUnitObject.FromEntity(fromUnitEntity),
                MutablePlayerUnitObject.FromEntity(toUnitEntity)
            );
        }

        private async Task<int[]> FindEmptySlots(IUnitsContainerObject container)
        {
            var existingUnitsInContainer = await PlayerUnitsRepository.GetAliveByContainerId(container.Id);
            return Enumerable.Range(0, container.Capacity)
                .Where(i => existingUnitsInContainer
                    .All(u => u.ContainerSlotIndex != i)
                ).ToArray();
        }
    }
}