using MageFactory.CombatContext.Contract;
using MageFactory.Flow.Contract;
using UnityEngine;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly CharacterAggregate character;

        internal CombatQueries(CharacterAggregate character) {
            this.character = character;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out IFlowItem item) {
            if (!character.getInventoryAggregate()
                    .tryGetItemAtCell(cell, out ICombatCharacterEquippedItem combatItem)) {
                item = null;
                return false;
            }

            item = combatItem;
            return true;
        }
    }
}