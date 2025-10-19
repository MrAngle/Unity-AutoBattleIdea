using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popup {
    public class PopupManager : MonoBehaviour {
        private Dictionary<PopupType, Queue<PopupBase>> _pools;
        [SerializeField] private const int InitialPoolPerType = 5;
        [SerializeField] private RectTransform parentCanvas;

        [SerializeField] private List<PopupPrefabEntry> popupPrefabs;
        public static PopupManager Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _pools = new Dictionary<PopupType, Queue<PopupBase>>();
            foreach (var entry in popupPrefabs) {
                var q = new Queue<PopupBase>();
                for (var i = 0; i < InitialPoolPerType; i++) {
                    var inst = Instantiate(entry.prefab, parentCanvas);
                    inst.gameObject.SetActive(false);
                    q.Enqueue(inst);
                }

                _pools[entry.type] = q;
            }
        }

        public PopupBase GetFromPool(PopupType type) {
            if (!_pools.ContainsKey(type)) {
                Debug.LogWarning($"PopupManager: brak puli dla typu {type}");
                return null;
            }

            var q = _pools[type];
            if (q.Count > 0) {
                var p = q.Dequeue();
                return p;
            }

            // utwórz nowy egzemplarz, jeśli pula pusta
            var prefab = popupPrefabs.Find(e => e.type == type).prefab;
            var inst = Instantiate(prefab, parentCanvas);
            inst.gameObject.SetActive(false);
            return inst;
        }

        public void ReturnToPool(PopupBase popup) {
            popup.gameObject.SetActive(false);
            // wpisz z powrotem do odpowiedniej kolejki
            // założenie: popup ma jakiś typ — musisz wiedzieć typ
            // możesz mieć pole popup.CurrentType lub coś takiego
            var t = /* pobierz typ z popup */ PopupType.Damage; // zmień wg implementacji
            _pools[t].Enqueue(popup);
        }

        /// Wygodna metoda „na zewnątrz”:
        public void ShowPopup(PopupType type, string textToShow, Vector2 pos, Color col) {
            var popupBase = GetFromPool(type);
            // popupBase.Show(textToShow, pos, type, col);
        }

        public void ShowDamage(int dmg, Vector2 pos, Color col) {
            ShowPopup(PopupType.Damage, "-" + dmg, pos, col);
        }

        [Serializable]
        public struct PopupPrefabEntry {
            public PopupType type;
            public PopupBase prefab;
        }
    }
}