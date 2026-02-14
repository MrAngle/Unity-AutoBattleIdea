using System.Collections.Generic;
using MageFactory.CombatContext.Contract;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.Inventory.Api {
    public class InventoryGridView : MonoBehaviour {
        [Header("Prefabs / Refs")] [SerializeField]
        private InventoryCellView cellPrefab;

        [SerializeField] private GridLayoutGroup gridLayout;

        private readonly Dictionary<Vector2Int, InventoryCellView> _views = new();

        private void Awake() {
            if (!gridLayout) gridLayout = GetComponent<GridLayoutGroup>();
        }

        public void build(ICombatInventory combatInventory) {
            clear();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = combatInventory.Width;

            for (int y = 0; y < combatInventory.Height; y++)
            for (int x = 0; x < combatInventory.Width; x++) {
                var coord = new Vector2Int(x, y);
                var v = Instantiate(cellPrefab, transform);
                v.Init(coord, combatInventory.getState(coord));
                _views[coord] = v;
            }
        }

        public void clear() {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            _views.Clear();
        }
    }
}