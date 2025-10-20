using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Slots.View {

    public class InventoryGridView : MonoBehaviour
    {
        [Header("Prefabs / Refs")]
        [SerializeField] private InventoryCellView cellPrefab;
        [SerializeField] private GridLayoutGroup gridLayout;

        private readonly Dictionary<Vector2Int, InventoryCellView> _views = new();

        private void Awake()
        {
            if (!gridLayout) gridLayout = GetComponent<GridLayoutGroup>();
        }

        public void Build(InventoryGrid model)
        {
            Clear();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = model.Width;

            for (int y = 0; y < model.Height; y++)
            for (int x = 0; x < model.Width; x++)
            {
                var coord = new Vector2Int(x, y);
                var v = Instantiate(cellPrefab, transform);
                v.Init(coord, model.GetState(coord));
                _views[coord] = v;
            }
        }

        public void Clear()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            _views.Clear();
        }
    }
}