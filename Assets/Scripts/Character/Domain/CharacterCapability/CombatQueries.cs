using MageFactory.CombatContext.Contract;
using UnityEngine;

namespace MageFactory.Character.Domain.CharacterCapability {
    internal class CombatQueries : ICombatQueries {
        private readonly ICombatCharacter character;

        internal CombatQueries(ICombatCharacter character) {
            this.character = character;
        }

        public bool tryGetItemAtCell(Vector2Int cell, out ICombatCharacterEquippedItem item) {
            return character.getInventoryAggregate().tryGetItemAtCell(cell, out item);
        }
    }
}