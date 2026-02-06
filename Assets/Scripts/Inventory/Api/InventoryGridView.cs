using System.Collections.Generic;
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

        public void Build(IInventoryGrid inventoryGrid) {
            Clear();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = inventoryGrid.Width;

            for (int y = 0; y < inventoryGrid.Height; y++)
            for (int x = 0; x < inventoryGrid.Width; x++) {
                var coord = new Vector2Int(x, y);
                var v = Instantiate(cellPrefab, transform);
                v.Init(coord, inventoryGrid.GetState(coord));
                _views[coord] = v;
            }
        }

        public void Clear() {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            _views.Clear();
        }
    }
}