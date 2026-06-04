using System.Collections.Generic;
using MageFactory.Character.Contract;
using MageFactory.CombatContextRuntime;
using MageFactory.Flow.Api;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;

namespace MageFactory.Character.Domain.CombatChar {
    internal sealed class CombatTickPlan {
        private readonly List<IFlowProcessor> flowSnapshot = new();
        private readonly List<CharacterCombatTickableItemAction> itemSnapshot = new();

        internal void executeCharacterTick(
            List<IFlowProcessor> activeFlows,
            IReadOnlyCollection<CharacterCombatTickableItemAction> tickableItemActions,
            CombatTicks combatTicks,
            Id<CharacterId> characterId,
            ICombatCapabilities combatCapabilities) {
            prepareSnapshot(activeFlows, tickableItemActions);
            executePreparedPhases(combatTicks, activeFlows, characterId, combatCapabilities);
        }

        private void prepareSnapshot(
            IReadOnlyList<IFlowProcessor> activeFlows,
            IReadOnlyCollection<CharacterCombatTickableItemAction> tickableItemActions) {
            flowSnapshot.Clear();
            itemSnapshot.Clear();

            for (var i = 0; i < activeFlows.Count; i++) {
                flowSnapshot.Add(activeFlows[i]);
            }

            foreach (CharacterCombatTickableItemAction tickableItemAction in tickableItemActions) {
                itemSnapshot.Add(tickableItemAction);
            }
        }

        private void executePreparedPhases(
            CombatTicks combatTicks,
            List<IFlowProcessor> activeFlows,
            Id<CharacterId> characterId,
            ICombatCapabilities combatCapabilities) {
            executeActiveFlowPhase(combatTicks);
            compactFinishedFlows(activeFlows);
            executeInventoryItemPhase(combatTicks, characterId, combatCapabilities);
        }

        private void executeActiveFlowPhase(CombatTicks combatTicks) {
            for (var i = 0; i < flowSnapshot.Count; i++) {
                flowSnapshot[i].tick(combatTicks);
            }
        }

        private void compactFinishedFlows(List<IFlowProcessor> activeFlows) {
            var nextActiveIndex = 0;

            for (var i = 0; i < activeFlows.Count; i++) {
                IFlowProcessor activeFlow = activeFlows[i];

                if (!activeFlow.isFinished()) {
                    activeFlows[nextActiveIndex] = activeFlow;
                    nextActiveIndex++;
                    continue;
                }
            }

            if (nextActiveIndex < activeFlows.Count) {
                activeFlows.RemoveRange(nextActiveIndex, activeFlows.Count - nextActiveIndex);
            }
        }

        private void executeInventoryItemPhase(
            CombatTicks combatTicks,
            Id<CharacterId> characterId,
            ICombatCapabilities combatCapabilities) {
            for (var i = 0; i < itemSnapshot.Count; i++) {
                itemSnapshot[i]?.Invoke(combatTicks, characterId, combatCapabilities);
            }
        }
    }
}