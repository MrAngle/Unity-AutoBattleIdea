using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain {
    internal class CombatCharacter : ICombatCharacter, IFlowOwner {
        private readonly Team team;
        private readonly CharacterAggregate characterAggregate;
        private readonly ICharacterCombatCapabilities characterCombatCapabilities;
        private readonly IFlowFactory flowFactory;

        internal CombatCharacter(CharacterAggregate characterAggregate,
                                 ICharacterCombatCapabilities characterCombatCapabilities,
                                 Team team,
                                 IFlowFactory flowFactory) {
            this.characterAggregate = NullGuard.NotNullOrThrow(characterAggregate);
            this.characterCombatCapabilities = NullGuard.NotNullOrThrow(characterCombatCapabilities);
            this.team = NullGuard.enumDefinedOrThrow(team);
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }


        public Id<CharacterId> getFlowOwnerCharacterId() {
            return characterAggregate.getId();
        }

        public Id<CharacterId> getId() {
            return characterAggregate.getId();
        }

        public ICombatCharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return characterAggregate.equipItemOrThrow(item);
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterAggregate.canPlaceItem(equipItemQuery);
        }

        public ICombatCharacterInventory getInventoryAggregate() {
            return characterAggregate.getInventoryAggregate();
        }

        public long getMaxHp() {
            return characterAggregate.getMaxHp();
        }

        public long getCurrentHp() {
            return characterAggregate.getCurrentHp();
        }

        public void apply(PowerAmount powerAmount) {
            characterAggregate.apply(powerAmount);
        }

        public string getName() {
            return characterAggregate.getName();
        }

        public void cleanup() {
            characterAggregate.cleanup();
        }

        // public void combatTick(IFlowConsumer flowConsumer) {
        //     characterAggregate.combatTick(flowConsumer, characterCombatCapabilities);
        // }

        public void combatTick(IFlowConsumer flowConsumer) {
            var entryPointsToTick = characterAggregate.getInventoryAggregate().getEntryPointsToTick();
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

        public ICharacterCombatCapabilities getCharacterCombatCapabilities() {
            return characterCombatCapabilities;
        }

        public Team getTeam() {
            return team;
        }
    }
}