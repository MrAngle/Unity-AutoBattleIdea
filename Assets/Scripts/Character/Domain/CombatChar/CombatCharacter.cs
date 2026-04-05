using MageFactory.Character.Domain.CharacterCapability;
using MageFactory.Character.Domain.FlowCapability;
using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacter : ICombatCharacter, IFlowOwner {
        private readonly Team team;
        private readonly CharacterAggregate characterAggregate;
        private readonly CharacterCombatCapabilities characterCombatCapabilities;
        private readonly IFlowFactory flowFactory;

        internal CombatCharacter(CharacterAggregate characterAggregate,
                                 CharacterCombatCapabilities characterCombatCapabilities,
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

        public long getMaxHp() {
            return characterAggregate.getMaxHp();
        }

        public long getCurrentHp() {
            return characterAggregate.getCurrentHp();
        }

        public string getName() {
            return characterAggregate.getName();
        }

        public void cleanup() {
            characterAggregate.cleanup();
        }

        public void combatTick(IFlowConsumer flowConsumer) {
            var entryPointsToTick = characterAggregate.getInventoryAggregate().getEntryPointsToTick();
            if (entryPointsToTick == null || entryPointsToTick.Count == 0)
                return;

            var router = GridAdjacencyRouter.create(characterCombatCapabilities.query());

            foreach (var entryPoint in entryPointsToTick) {
                if (entryPoint == null) {
                    continue;
                }

                var flow = flowFactory.create(new CombatCharacterEquippedEntryPointItem(entryPoint), router,
                    flowConsumer, new FlowCapabilities(characterCombatCapabilities), this);
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