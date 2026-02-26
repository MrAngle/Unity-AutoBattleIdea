#nullable enable
using System;
using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate : IFlowOwner {
        private readonly Id<CharacterId> characterId;
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventory;
        private readonly Team team;
        private readonly IFlowFactory flowFactory;
        private readonly ICharacterEventPublisher characterEventPublisher;

        private CharacterAggregate(
            CharacterData data,
            ICharacterInventory characterInventoryFacade,
            Team team,
            IFlowFactory flowFactory,
            ICharacterEventPublisher characterEventPublisher
        ) {
            characterId = new Id<CharacterId>(IdGenerator.Next());
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            characterData = NullGuard.NotNullOrThrow(data);
            characterInventory = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.team = NullGuard.enumDefinedOrThrow(team);
            this.characterEventPublisher = NullGuard.NotNullOrThrow(characterEventPublisher);

            characterData.OnHpChanged += handleCharacterDataHpChanged;
        }

        public static CharacterAggregate createFrom(
            CreateCombatCharacterCommand characterCreateCommand,
            ICharacterInventory characterInventoryFacade, // I think it will be better when a character creates inventory
            IFlowFactory flowFactory,
            ICharacterEventPublisher characterEventPublisher
        ) {
            return new CharacterAggregate(CharacterData.from(characterCreateCommand), characterInventoryFacade,
                characterCreateCommand.team, flowFactory, characterEventPublisher);
        }

        public Id<CharacterId> getId() {
            return characterId;
        }

        public Id<CharacterId> getFlowOwnerCharacterId() {
            return getId();
        }

        public string getName() {
            return characterData.getName();
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return characterInventory;
        }

        public long getMaxHp() {
            return characterData.getMaxHp();
        }

        public long getCurrentHp() {
            return characterData.CurrentHp;
        }

        public void apply(PowerAmount powerAmount) {
            characterData.applyDamage(powerAmount);
            if (characterData.CurrentHp <= 0) {
                characterEventPublisher.publish(new CharacterDeathDtoEvent(characterId));
            }
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterInventory.canPlace(new PlaceItemQuery(equipItemQuery.itemDefinition,
                equipItemQuery.origin));
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand equipItemCommand) {
            if (!canPlaceItem(new EquipItemQuery(equipItemCommand.itemDefinition, equipItemCommand.origin))) {
                throw new ArgumentException("Cannot equip item");
            }

            return characterInventory.place(
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin));
        }

        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        public void combatTick(IFlowConsumer flowConsumer, ICharacterCombatCapabilities characterCombatCapabilities) {
            var entryPointsToTick = characterInventory.getEntryPointsToTick();
            if (entryPointsToTick == null || entryPointsToTick.Count == 0)
                return;

            var router = GridAdjacencyRouter.create(characterCombatCapabilities.query());

            foreach (var entryPoint in entryPointsToTick) {
                if (entryPoint == null) {
                    continue;
                }

                var flow = flowFactory.create(entryPoint, router, flowConsumer, this);
                flow.start();
            }
        }

        public Team getTeam() {
            return team;
        }

        ~CharacterAggregate() {
            // finalizer — w razie gdyby ktoś zapomniał Cleanup (ale nie polegaj na tym)
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        private void handleCharacterDataHpChanged(CharacterData data, long newHp, long previousHpValue) {
            characterEventPublisher.publish(new CharacterHpChangedDtoEvent(characterId, newHp, previousHpValue));
        }
    }
}