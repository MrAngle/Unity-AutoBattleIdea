using System;
using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.Character.Domain.CombatChar.CharCombatEventProcessors;
using MageFactory.Character.Domain.FlowCapability;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Api;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacter : IFlowOwner {
        private readonly CombatCharacterData combatCharacterData;
        private readonly CharacterAggregate characterAggregate;
        private readonly IFlowFactory flowFactory;
        private readonly CharacterCombatEventProcessor characterCombatEventProcessor;
        private readonly List<IFlowProcessor> activeFlows = new();
        private readonly Dictionary<Id<ItemId>, CombatPlacedItemRuntimeState> runtimeStateByItemId = new();
        private readonly Dictionary<Id<ItemId>, IFlowItem> flowItemByItemId = new();
        private readonly CombatTickPlan combatTickPlan = new();
        private int createdFlowCount;

        internal CombatCharacter(CharacterAggregate characterAggregate,
                                 Team team,
                                 IFlowFactory flowFactory,
                                 CharacterCombatEventProcessor characterCombatEventProcessor) {
            this.characterAggregate = NullGuard.NotNullOrThrow(characterAggregate);
            this.combatCharacterData =
                NullGuard.NotNullOrThrow(new CombatCharacterData(this.characterAggregate.getCharacterInfo(), team));
            this.flowFactory = NullGuard.NotNullOrThrow(flowFactory);
            this.characterCombatEventProcessor = NullGuard.NotNullOrThrow(characterCombatEventProcessor);

            initializeRuntimeStateForEquippedItems();
        }

        public Id<CharacterId> getFlowOwnerCharacterId() {
            return combatCharacterData.getCharacterId();
        }

        public void cleanup() {
            characterAggregate.cleanup();
        }

        public void combatTick(CombatTicks combatTicks, ICombatCapabilities combatCapabilities) {
            combatTickPlan.executeCharacterTick(
                activeFlows,
                characterAggregate.getInventoryAggregate().getTickableItems(),
                combatTicks,
                combatCharacterData.getCharacterId(),
                combatCapabilities);
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

        public DamageTaken takeDamage(ResolvedDamage resolvedDamage) {
            return characterAggregate.takeDamage(resolvedDamage);
        }

        public bool tryMoveItem(ICharacterEquippedItem characterEquippedItem) {
            return characterAggregate.tryMoveItem(characterEquippedItem);
        }

        public ICharacterEquippedItem equipItemOrThrow(EquipItemCommand item) {
            ICharacterEquippedItem equippedItem = characterAggregate.equipItemOrThrow(item);
            registerRuntimeStateFor(equippedItem);
            return equippedItem;
        }

        internal bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (this.getInventoryAggregate()
                .tryGetItemAtCell(cell, out ICharacterEquippedItem combatItem)) {
                item = getFlowItemOrThrow(combatItem.getId());
                return true;
            }

            item = null;
            return false;
        }

        internal void createFlow(Id<ItemId> entryPointItemId,
                                 IFlowConsumer flowConsumer,
                                 ICombatCapabilities combatCapabilities) {
            NullGuard.ValidIdOrThrow(entryPointItemId);
            NullGuard.NotNullOrThrow(flowConsumer);
            NullGuard.NotNullOrThrow(combatCapabilities);

            if (!characterAggregate.getInventoryAggregate()
                    .tryGetEntryPointById(entryPointItemId, out ICharacterEquippedEntryPoint entryPoint)) {
                throw new InvalidOperationException(
                    $"EntryPoint with id '{entryPointItemId}' was not found in character inventory.");
            }

            IFlowItem entryPointFlowItem = getFlowItemOrThrow(entryPointItemId);
            if (!canProcessFlowItem(entryPointFlowItem)) {
                return;
            }

            IFlowRouter router = ProcessableGridAdjacencyRouter.create(
                tryGetItemAtCell,
                canProcessFlowItem);

            IFlowProcessor flow = flowFactory.create(new FlowCreationCommand(
                new Id<ActiveFlowId>(IdGenerator.Next()),
                entryPoint.getFlowKind(),
                entryPointFlowItem,
                router,
                flowConsumer,
                new FlowCapabilities(this),
                this));

            activeFlows.Add(flow);
            createdFlowCount++;
        }

        public void consumeCombatEvent(CombatEvent combatEvent) {
            characterCombatEventProcessor.process(this, combatEvent);
        }

        public int getActiveFlowCount() {
            return activeFlows.Count;
        }

        public int getCreatedFlowsInCurrentBattleCount() {
            return createdFlowCount;
        }

        public int getActiveFlowCountOnItem(Id<ItemId> itemId) {
            NullGuard.ValidIdOrThrow(itemId);

            return runtimeStateByItemId.TryGetValue(itemId, out CombatPlacedItemRuntimeState runtimeState)
                ? runtimeState.getActiveFlowCount()
                : 0;
        }

        public bool canProcessFlowItem(IFlowItem item) {
            return getRuntimeStateOrThrow(item).canAcceptFlow();
        }

        public ItemFlowProcessingSlot startProcessingFlowItem(IFlowItem item) {
            return getRuntimeStateOrThrow(item).startProcessingFlow();
        }

        public void finishProcessingFlowItem(ItemFlowProcessingSlot processingSlot) {
            ItemFlowProcessingSlot slot = NullGuard.NotNullOrThrow(processingSlot);
            Id<ItemId> itemId = slot.getItemId();

            if (!runtimeStateByItemId.TryGetValue(itemId, out CombatPlacedItemRuntimeState runtimeState)) {
                throw new InvalidOperationException(
                    $"Cannot finish processing item '{itemId}' because no active flow is processing it.");
            }

            runtimeState.finishProcessingFlow(slot);
        }

        public void collectActiveFlowStates(IActiveFlowStateCollector collector) {
            NullGuard.NotNullOrThrow(collector);

            for (int i = 0; i < activeFlows.Count; i++) {
                activeFlows[i].collectActiveFlowStates(collector);
            }
        }

        private void initializeRuntimeStateForEquippedItems() {
            foreach (IGridItemPlaced placedItem in characterAggregate
                         .getInventoryAggregate()
                         .getPlacedSnapshot()) {
                if (!characterAggregate.getInventoryAggregate()
                        .tryGetItemById(placedItem.getId(), out ICharacterEquippedItem equippedItem)) {
                    throw new InvalidOperationException(
                        $"Cannot initialize runtime state because item '{placedItem.getId()}' was not found.");
                }

                registerRuntimeStateFor(equippedItem);
            }
        }

        private void registerRuntimeStateFor(ICharacterEquippedItem item) {
            ICharacterEquippedItem equippedItem = NullGuard.NotNullOrThrow(item);
            Id<ItemId> itemId = equippedItem.getId();

            if (runtimeStateByItemId.ContainsKey(itemId)) {
                throw new InvalidOperationException(
                    $"Runtime state for item '{itemId}' is already initialized.");
            }

            IFlowItem flowItem = new CombatCharacterEquippedItem(equippedItem);
            flowItemByItemId[itemId] = flowItem;
            runtimeStateByItemId[itemId] = new CombatPlacedItemRuntimeState(flowItem);
        }

        private IFlowItem getFlowItemOrThrow(Id<ItemId> itemId) {
            Id<ItemId> validItemId = NullGuard.ValidIdOrThrow(itemId);

            if (flowItemByItemId.TryGetValue(validItemId, out IFlowItem flowItem)) {
                return flowItem;
            }

            throw new InvalidOperationException(
                $"Flow item '{validItemId}' has no initialized runtime state.");
        }

        private CombatPlacedItemRuntimeState getRuntimeStateOrThrow(IFlowItem item) {
            IFlowItem flowItem = NullGuard.NotNullOrThrow(item);
            Id<ItemId> itemId = flowItem.getId();

            if (runtimeStateByItemId.TryGetValue(itemId, out CombatPlacedItemRuntimeState runtimeState)) {
                return runtimeState;
            }

            throw new InvalidOperationException(
                $"Runtime state for item '{itemId}' was not initialized before flow processing.");
        }
    }
}