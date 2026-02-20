#nullable enable
using System;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate : ICombatCharacter {
        private readonly Id<CharacterId> characterId;
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventory;
        private readonly Team team;
        private readonly ICharacterCombatCapabilities characterCombatCapabilities;
        private readonly IFlowFactory flowFactory;

        private CharacterAggregate(
            CharacterData data,
            ICharacterInventory characterInventoryFacade,
            Team team,
            ICharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory,
            IFlowFactory flowFactory
        ) {
            characterId = new Id<CharacterId>(IdGenerator.Next());
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            characterData = NullGuard.NotNullOrThrow(data);
            characterInventory = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.team = NullGuard.enumDefinedOrThrow(team);


            characterData.OnHpChanged += handleCharacterDataHpChanged;

            // Be warned: use "this" in constructor. It's not a good practice, but Im not so similar to C#, but it should work
            characterCombatCapabilities = characterCombatCapabilitiesFactory.createCombatContextFactory(this);
        }

        public static CharacterAggregate createFrom(
            CreateCombatCharacterCommand characterCreateCommand,
            ICharacterInventory characterInventoryFacade, // I think it will be better when a character creates inventory
            ICharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory,
            IFlowFactory flowFactory
        ) {
            return new CharacterAggregate(CharacterData.from(characterCreateCommand), characterInventoryFacade,
                characterCreateCommand.team, characterCombatCapabilitiesFactory, flowFactory);
        }

        public Id<CharacterId> getId() {
            return characterId;
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

        public event Action<ICombatCharacter, long, long> OnHpChanged;
        public event Action<ICombatCharacter> OnDeath;

        public void apply(PowerAmount powerAmount) {
            characterData.applyDamage(powerAmount);
            if (characterData.CurrentHp <= 0) OnDeath?.Invoke(this);
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
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin,
                    characterCombatCapabilities));
        }

        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
        }

        public void combatTick() {
            var entryPointsToTick = characterInventory.getEntryPointsToTick();
            if (entryPointsToTick == null || entryPointsToTick.Count == 0)
                return;

            var router = GridAdjacencyRouter.create(characterCombatCapabilities.query());

            foreach (var entryPoint in entryPointsToTick) {
                if (entryPoint == null) {
                    continue;
                }

                var flow = flowFactory.create(entryPoint, router);
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
            OnHpChanged?.Invoke(this, newHp, previousHpValue);
        }
    }
}