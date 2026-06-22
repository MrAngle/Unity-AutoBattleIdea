using System;
using System.Collections.Generic;
using MageFactory.ActionEffect;
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
        private readonly List<Id<ItemId>> eventTriggeredDefensiveEntryPointIds = new();
        private readonly Dictionary<CombatEventType, int> combatEventCountsByType = new();
        private readonly Dictionary<Id<ItemId>, CombatPlacedItemRuntimeState> runtimeStateByItemId = new();
        private readonly Dictionary<Id<ItemId>, IFlowItem> flowItemByItemId = new();
        private readonly CombatTickPlan combatTickPlan = new();
        private readonly CombatGuardState guardState = new();
        private int createdFlowCount;
        private int nextEventTriggeredDefensiveEntryPointIndex;

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

        public DamageTaken applyResolvedDamage(ResolvedDamage resolvedDamage) {
            if (guardState.getPreparedGuardCount() == 0) {
                return takeDamage(resolvedDamage);
            }

            ResolvedDamage damageAfterGuard = guardState.applyTo(
                resolvedDamage,
                out GuardDamageApplicationResult guardDamageApplicationResult);
            characterAggregate.publishGuardAbsorbedDamage(guardDamageApplicationResult);
            return takeDamage(damageAfterGuard);
        }

        public bool tryAddGuardPower(GuardPower guardPower, out PreparedGuardAddResult guardAddResult) {
            return guardState.tryAddGuard(guardPower, out guardAddResult);
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

        internal bool createFlow(Id<ItemId> entryPointItemId,
                                 IFlowConsumer flowConsumer,
                                 ICombatCapabilities combatCapabilities) {
            return tryCreateFlow(
                entryPointItemId,
                flowConsumer,
                combatCapabilities,
                PowerAmount.noPower(),
                default);
        }

        internal bool tryCreateDefensiveFlowFor(IncomingAttackDamageCombatEvent combatEvent,
                                                IFlowConsumer flowConsumer,
                                                ICombatCapabilities combatCapabilities) {
            NullGuard.NotNullOrThrow(combatEvent);

            int entryPointCount = eventTriggeredDefensiveEntryPointIds.Count;
            if (entryPointCount == 0) {
                return false;
            }

            clampDefensiveEntryPointCursor(entryPointCount);
            int roundRobinStartIndex = nextEventTriggeredDefensiveEntryPointIndex;
            for (int offset = 0; offset < entryPointCount; offset++) {
                int entryPointIndex = getRoundRobinDefensiveEntryPointIndex(
                    roundRobinStartIndex,
                    offset,
                    entryPointCount);

                if (tryCreateDefensiveFlowFromEntryPointAtIndex(
                        entryPointIndex,
                        flowConsumer,
                        combatCapabilities,
                        combatEvent)) {
                    advanceDefensiveEntryPointCursorAfter(entryPointIndex, entryPointCount);
                    return true;
                }
            }

            return false;
        }

        private void clampDefensiveEntryPointCursor(int entryPointCount) {
            if (nextEventTriggeredDefensiveEntryPointIndex >= entryPointCount) {
                nextEventTriggeredDefensiveEntryPointIndex = 0;
            }
        }

        private static int getRoundRobinDefensiveEntryPointIndex(
            int startIndex,
            int offset,
            int entryPointCount) {
            return (startIndex + offset) % entryPointCount;
        }

        private bool tryCreateDefensiveFlowFromEntryPointAtIndex(
            int entryPointIndex,
            IFlowConsumer flowConsumer,
            ICombatCapabilities combatCapabilities,
            IncomingAttackDamageCombatEvent combatEvent) {
            return tryCreateFlow(
                eventTriggeredDefensiveEntryPointIds[entryPointIndex],
                flowConsumer,
                combatCapabilities,
                combatEvent.getRawDamageToReceive(),
                combatEvent.getSourceCharacterId());
        }

        private void advanceDefensiveEntryPointCursorAfter(int usedEntryPointIndex, int entryPointCount) {
            nextEventTriggeredDefensiveEntryPointIndex = (usedEntryPointIndex + 1) % entryPointCount;
        }

        private bool tryCreateFlow(Id<ItemId> entryPointItemId,
                                   IFlowConsumer flowConsumer,
                                   ICombatCapabilities combatCapabilities,
                                   PowerAmount initialAttackPower,
                                   Id<CharacterId> sourceCharacterId) {
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
                return false;
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
                this,
                NullGuard.NotNullOrThrow(initialAttackPower),
                sourceCharacterId));

            activeFlows.Add(flow);
            createdFlowCount++;
            return true;
        }

        public void consumeCombatEvent(CombatEvent combatEvent,
                                       IFlowConsumer flowConsumer,
                                       ICombatCapabilities combatCapabilities) {
            recordCombatEvent(combatEvent);
            characterCombatEventProcessor.process(this, combatEvent, flowConsumer, combatCapabilities);
        }

        public int getActiveFlowCount() {
            return activeFlows.Count;
        }

        public int getCreatedFlowsInCurrentBattleCount() {
            return createdFlowCount;
        }

        public int getCombatEventCount(CombatEventType combatEventType) {
            NullGuard.enumDefinedOrThrow(combatEventType);

            return combatEventCountsByType.TryGetValue(combatEventType, out int count)
                ? count
                : 0;
        }

        public int getPreparedGuardCount() {
            return guardState.getPreparedGuardCount();
        }

        public long getTotalPreparedGuardPower() {
            return guardState.getTotalPreparedGuardPower();
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

        public void collectPreparedGuardStates(IPreparedGuardStateCollector collector) {
            guardState.collectPreparedGuardStates(collector);
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

            if (equippedItem is ICharacterEquippedEntryPoint entryPoint
                && entryPoint.getFlowKind() == FlowKind.Defense
                && entryPoint.getTriggerKind() == EntryPointTriggerKind.CombatEvent
                && entryPoint.getCombatHook().observes(CombatEventType.INCOMING_ATTACK_DAMAGE)) {
                eventTriggeredDefensiveEntryPointIds.Add(itemId);
            }
        }

        private void recordCombatEvent(CombatEvent combatEvent) {
            CombatEvent eventToRecord = NullGuard.NotNullOrThrow(combatEvent);
            CombatEventType eventType = eventToRecord.getType();

            combatEventCountsByType.TryGetValue(eventType, out int currentCount);
            combatEventCountsByType[eventType] = currentCount + 1;
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