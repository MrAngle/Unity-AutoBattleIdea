using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.Character.Contract;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model.Shape;
using UnityEngine;

namespace MageFactory.Character.Domain.CombatChar {
    internal class CombatCharacterEquippedItem : ICombatCharacterEquippedItem {
        private readonly ICharacterEquippedItem characterEquippedItem;

        public CombatCharacterEquippedItem(ICharacterEquippedItem characterEquippedItem) {
            this.characterEquippedItem = characterEquippedItem;
        }

        public Id<ItemId> getId() {
            return characterEquippedItem.getId();
        }

        public Vector2Int getOrigin() {
            return characterEquippedItem.getOrigin();
        }

        public IReadOnlyCollection<Vector2Int> getOccupiedCells() {
            return characterEquippedItem.getOccupiedCells();
        }

        public ShapeArchetype getShape() {
            return characterEquippedItem.getShape();
        }

        public IActionDescription prepareItemActionDescription() {
            return characterEquippedItem.prepareItemActionDescription();
        }
    }
}