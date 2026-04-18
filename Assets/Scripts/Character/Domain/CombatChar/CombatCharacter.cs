using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.FlowCapability;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacter : IFlowOwner {
        private readonly CombatCharacterData combatCharacterData;
        private readonly CharacterAggregate characterAggregate;
        private readonly IFlowFactory flowFactory;

        internal CombatCharacter(CharacterAggregate characterAggregate,
                                 Team team,
                                 IFlowFactory flowFactory) {
            this.characterAggregate = NullGuard.NotNullOrThrow(characterAggregate);
            this.combatCharacterData =
                NullGuard.NotNullOrThrow(new CombatCharacterData(this.characterAggregate.getCharacterInfo(), team));
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
        }

        public Id<CharacterId> getFlowOwnerCharacterId() {
            return combatCharacterData.getCharacterId();
        }

        public void cleanup() {
            characterAggregate.cleanup();
        }

        public void combatTick(IFlowConsumer flowConsumer) {
            IReadOnlyCollection<ICharacterEquippedEntryPointToTick> entryPointsToTick =
                characterAggregate.getInventoryAggregate().getEntryPointsToTick();
            if (entryPointsToTick == null || entryPointsToTick.Count == 0)
                return;

            var router = GridAdjacencyRouter.create(tryGetItemAtCell);

            foreach (ICharacterEquippedEntryPointToTick entryPoint in entryPointsToTick) {
                if (entryPoint == null) {
                    continue;
                }

                var flow = flowFactory.create(
                    entryPoint.getFlowKind(),
                    new CombatCharacterEquippedEntryPointItem(entryPoint),
                    router,
                    flowConsumer,
                    new FlowCapabilities(this),
                    this);
                flow.start();
            }
        }

        public IReadOnlyCombatCharacterData getCharacterInfo() {
            return combatCharacterData;
        }

        public bool canPlaceItem(EquipItemQuery equipItemQuery) {
            return characterAggregate.canPlaceItem(equipItemQuery);
        }

        public ICharacterInventory getInventoryAggregate() {
            return characterAggregate.getInventoryAggregate();
        }

        public void takeDamage(DamageToReceive powerAmount) {
            characterAggregate.takeDamage(powerAmount);
        }

        public bool tryMoveItem(ICharacterEquippedItem characterEquippedItem) {
            return characterAggregate.tryMoveItem(characterEquippedItem);
        }

        public ICharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            return characterAggregate.equipItemOrThrow(item);
        }

        internal bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (this.getInventoryAggregate()
                .tryGetItemAtCell(cell, out ICharacterEquippedItem combatItem)) {
                item = new CombatCharacterEquippedItem(combatItem);
                return true;
            }

            item = null;
            return false;
        }
    }
}