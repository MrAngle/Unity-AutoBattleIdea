// using System.Reflection;
// using UnityEngine;
//
// namespace MageFactory.Inventory.Controller {
//     internal sealed class InventoryPrefabInitializer {
//         private readonly CellViewPrefabInventoryCellView cellViewPrefab;
//         private readonly GridViewPrefabInventoryGridView gridViewPrefab;
//
//         public InventoryPrefabInitializer(
//             CellViewPrefabInventoryCellView cellViewPrefab,
//             GridViewPrefabInventoryGridView gridViewPrefab
//         ) {
//             this.cellViewPrefab = cellViewPrefab;
//             this.gridViewPrefab = gridViewPrefab;
//         }
//
//         public InventoryGridView createGridView(Transform parent) {
//             var view = Object.Instantiate(gridViewPrefab.Get(), parent, false);
//
//             // TEMP: reflection jak musisz
//             var fieldInfo = view.GetType().GetField("cellPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
//             if (fieldInfo != null && fieldInfo.GetValue(view) == null)
//                 fieldInfo.SetValue(view, cellViewPrefab.Get());
//
//             var rt = (RectTransform)view.transform;
//             rt.anchorMin = Vector2.zero;
//             rt.anchorMax = Vector2.one;
//             rt.offsetMin = Vector2.zero;
//             rt.offsetMax = Vector2.zero;
//
//             return view;
//         }
//     }
// }

