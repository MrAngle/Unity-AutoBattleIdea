#nullable enable
using System;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CharacterAggregate : ICombatCharacter {
        private readonly CharacterData characterData;
        private readonly ICharacterInventory characterInventoryFacade;
        private readonly Team team;
        private readonly ICharacterCombatCapabilities characterCombatCapabilities;

        private CharacterAggregate(
            CharacterData data,
            ICharacterInventory characterInventoryFacade,
            Team team,
            ICharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory
        ) {
            characterData = NullGuard.NotNullOrThrow(data);
            this.characterInventoryFacade = NullGuard.NotNullOrThrow(characterInventoryFacade);
            this.team = NullGuard.enumDefinedOrThrow(team);

            characterData.OnHpChanged += handleCharacterDataHpChanged;

            // Be warned: use "this" in constructor. It's not a good practice, but Im not so similar to C#, but it should work
            characterCombatCapabilities = characterCombatCapabilitiesFactory.createCombatContextFactory(this);
        }

        public static CharacterAggregate createFrom(
            CreateCombatCharacterCommand characterCreateCommand,
            ICharacterInventory characterInventoryFacade, // I think it will be better when a character creates inventory
            ICharacterCombatCapabilitiesFactory characterCombatCapabilitiesFactory
        ) {
            return new CharacterAggregate(CharacterData.from(characterCreateCommand), characterInventoryFacade,
                characterCreateCommand.team, characterCombatCapabilitiesFactory);
        }


        public string getName() {
            return characterData.getName();
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return characterInventoryFacade;
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
            return characterInventoryFacade.canPlace(new PlaceItemQuery(equipItemQuery.itemDefinition,
                equipItemQuery.origin));
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand equipItemCommand) {
            if (!canPlaceItem(new EquipItemQuery(equipItemCommand.itemDefinition, equipItemCommand.origin))) {
                throw new ArgumentException("Cannot equip item");
            }

            return characterInventoryFacade.place(
                new PlaceItemCommand(equipItemCommand.itemDefinition, equipItemCommand.origin,
                    characterCombatCapabilities));
        }

        public void cleanup() {
            characterData.OnHpChanged -= handleCharacterDataHpChanged;
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