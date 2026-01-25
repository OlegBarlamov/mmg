using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.ArtifactTypes;
using Epic.Data.Artifact;
using Epic.Data.Artifacts;
using JetBrains.Annotations;

namespace Epic.Core.Services.Artifacts
{
    [UsedImplicitly]
    public class DefaultArtifactsService : IArtifactsService
    {
        public IArtifactsRepository ArtifactsRepository { get; }
        public IArtifactTypesService ArtifactTypesService { get; }

        public DefaultArtifactsService(
            [NotNull] IArtifactsRepository artifactsRepository,
            [NotNull] IArtifactTypesService artifactTypesService)
        {
            ArtifactsRepository = artifactsRepository ?? throw new ArgumentNullException(nameof(artifactsRepository));
            ArtifactTypesService = artifactTypesService ?? throw new ArgumentNullException(nameof(artifactTypesService));
        }

        public async Task<IArtifactObject> GetById(Guid id)
        {
            var entity = await ArtifactsRepository.GetById(id);
            var obj = MutableArtifactObject.FromEntity(entity);
            obj.ArtifactType = await ArtifactTypesService.GetById(obj.TypeId);
            return obj;
        }

        public async Task<IArtifactObject[]> GetByHeroId(Guid heroId)
        {
            var entities = await ArtifactsRepository.GetByHeroId(heroId);
            var objects = entities.Select(MutableArtifactObject.FromEntity).ToArray();
            await Task.WhenAll(objects.Select(async o => o.ArtifactType = await ArtifactTypesService.GetById(o.TypeId)));
            return objects.ToArray<IArtifactObject>();
        }

        public async Task<IArtifactObject> Create(Guid heroId, Guid typeId)
        {
            // Validate type exists (helps avoid dangling references).
            var type = await ArtifactTypesService.GetById(typeId);

            var id = Guid.NewGuid();
            var entity = await ArtifactsRepository.Create(id, new ArtifactFields
            {
                HeroId = heroId,
                TypeId = typeId,
                EquippedSlotsIndexes = Array.Empty<int>(),
            });
            var obj = MutableArtifactObject.FromEntity(entity);
            obj.ArtifactType = type;
            return obj;
        }

        public async Task<IArtifactObject> EquipArtifact(Guid heroId, IArtifactObject artifact, int[] equippedSlotsIndexes)
        {
            if (artifact == null)
                throw new ArgumentNullException(nameof(artifact));

            var artifactId = artifact.Id;
            var targetEntity = await ArtifactsRepository.GetById(artifactId);
            // Validate ownership using stored entity (not just the passed object).
            if (targetEntity.HeroId != heroId)
                throw new InvalidOperationException($"Artifact {artifactId} does not belong to hero {heroId}");

            // Validate type exists (helps avoid dangling references).
            var artifactType = await ArtifactTypesService.GetById(targetEntity.TypeId);

            var normalizedIndexes = (equippedSlotsIndexes ?? Array.Empty<int>())
                .Where(i => i >= 0)
                .Distinct()
                .ToArray();

            // Empty -> unequip (no slots occupied)
            if (normalizedIndexes.Length == 0)
            {
                var unequipped = MutableArtifactObject.FromEntity(targetEntity);
                unequipped.EquippedSlotsIndexes = Array.Empty<int>();
                await ArtifactsRepository.Update(unequipped.ToEntity());
                return unequipped;
            }

            // Resolve requested slot types and validate indexes exist.
            var requestedSlotTypes = new List<ArtifactSlot>(normalizedIndexes.Length);
            foreach (var idx in normalizedIndexes)
            {
                if (!ArtifactEquipmentSlots.TryGetSlotType(idx, out var slotType) || slotType == ArtifactSlot.None)
                    throw new InvalidOperationException($"Invalid equipment slot index {idx}");
                requestedSlotTypes.Add(slotType);
            }

            // Equipped items must match the artifact type's required slots (order-insensitive).
            // Bag is treated as a regular slot type (can be mixed with others).
            var requiredSlots = (artifactType.Slots ?? Array.Empty<ArtifactSlot>())
                .Where(x => x != ArtifactSlot.None)
                .ToArray();

            if (requiredSlots.Length == 0)
                throw new InvalidOperationException($"Artifact type {artifactType.Id} does not define any equipment slots");

            if (requiredSlots.Length != requestedSlotTypes.Count)
                throw new InvalidOperationException(
                    $"Artifact type {artifactType.Id} requires {requiredSlots.Length} slot(s), but {requestedSlotTypes.Count} provided");

            bool MatchesMultiset(IEnumerable<ArtifactSlot> a, IEnumerable<ArtifactSlot> b) =>
                a.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count())
                    .OrderBy(k => k.Key)
                    .SequenceEqual(
                        b.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count())
                            .OrderBy(k => k.Key));

            if (!MatchesMultiset(requiredSlots, requestedSlotTypes))
                throw new InvalidOperationException(
                    $"Artifact type {artifactType.Id} can not be equipped to provided slot(s): {string.Join(", ", requestedSlotTypes)}");

            // Validate slot availability (no other artifact occupies the target indexes).
            var heroArtifacts = await ArtifactsRepository.GetByHeroId(heroId);
            var occupied = heroArtifacts
                .Where(a => a.Id != artifactId)
                .SelectMany(a => (a.EquippedSlotsIndexes ?? Array.Empty<int>()).Select(i => new { Index = i, ArtifactId = a.Id }))
                .Where(x => x.Index >= 0)
                .ToDictionary(x => x.Index, x => x.ArtifactId);

            foreach (var idx in normalizedIndexes)
            {
                if (occupied.TryGetValue(idx, out var occupiedBy))
                    throw new InvalidOperationException($"Equipment slot index {idx} is already occupied by artifact {occupiedBy}");
            }

            var targetMutable = MutableArtifactObject.FromEntity(targetEntity);
            targetMutable.EquippedSlotsIndexes = normalizedIndexes;
            await ArtifactsRepository.Update(targetMutable.ToEntity());

            var updated = await ArtifactsRepository.GetById(artifactId);
            var updatedObj = MutableArtifactObject.FromEntity(updated);
            updatedObj.ArtifactType = artifactType;
            return updatedObj;
        }

        public Task Update(IArtifactObject artifact)
        {
            return ArtifactsRepository.Update(artifact.ToEntity());
        }

        public Task Remove(Guid id)
        {
            return ArtifactsRepository.Remove(id);
        }
    }
}

