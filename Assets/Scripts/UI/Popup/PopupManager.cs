using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }

        [Header("Parent (root Canvas)")]
        [SerializeField] private Canvas parentCanvas;            // główny Canvas sceny
        [SerializeField] private RectTransform parentRect;       // jego RectTransform

        [Header("Prefab & Pool")]
        [SerializeField] private PopupBase popupPrefab;          // prefab pojedynczego popupu (bez Canvas!)
        [SerializeField] private int initialPoolSize = 10;

        private readonly Queue<PopupBase> _pool = new();
        private Camera _uiCam = null; // for now

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // Auto-wire jeśli nie ustawiono w Inspektorze
            // if (!parentCanvas) parentCanvas = FindObjectOfType<Canvas>();
            // if (!parentRect && parentCanvas) parentRect = (RectTransform)parentCanvas.transform;

            // for now - poki jest overlay
            // _uiCam = parentCanvas && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            //     ? null
            //     : (parentCanvas ? parentCanvas.worldCamera : null);

            // Wstępne wypełnienie puli
            for (int i = 0; i < initialPoolSize; i++)
            {
                var p = Instantiate(popupPrefab, parentRect);
                p.gameObject.SetActive(false);
                _pool.Enqueue(p);
            }
        }

        private PopupBase GetPopupBase()
        {
            if (_pool.Count > 0)
            {
                var p = _pool.Dequeue();
                p.gameObject.SetActive(true);
                return p;
            }
            return Instantiate(popupPrefab, parentRect);
        }

        public void ReturnToPool(PopupBase popup)
        {
            popup.gameObject.SetActive(false);
            _pool.Enqueue(popup);
        }

        public void ShowHPChangeDamage(Component ownerTransform, int hpChange) {
            if (hpChange > 0) {
                Show(ownerTransform, $"+{hpChange}", Color.green, offset: null, moveY: 50f, duration: 2f);
            }
            else if (hpChange < 0) {
                Show(ownerTransform, $"{hpChange}", Color.red, offset: null, moveY: 50f, duration: 2f);
            }
            else {
                Show(ownerTransform, "0", Color.white, offset: null, moveY: 50f, duration: 2f);
            }
        }

        /// <summary>
        /// Pokaż popup na środku komponentu, który wywołuje (UI: środek Recta, inne: Transform.position).
        /// </summary>
        public void Show(Component owner, string textToShow, Color color, Vector2? offset = null, float moveY = 50f, float duration = 1f) {
            Show(owner.transform, textToShow, color, offset, moveY, duration);
        }
        
        public void Show(Transform ownerTransform, string textToShow, Color color, Vector2? offset = null, float moveY = 200f, float duration = 1f)
        {
            // Vector3 worldPos = ownertransform is RectTransform rectTransform
            //     ? rectTransform.TransformPoint(rectTransform.rect.center)
            //     : ownertransform.position;
            Vector3 worldPos;
            if (ownerTransform is RectTransform rectTransform)
                worldPos = rectTransform.TransformPoint(rectTransform.rect.center);
            else
                worldPos = ownerTransform.position;

            // przelicz na ekran + lokalne UI
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_uiCam, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, _uiCam,
                out Vector2 localPos);

            // dodaj offset w lokalnych współrzędnych UI
            Vector2 finalPos = localPos + (offset ?? Vector2.zero);

            Show(textToShow, finalPos + (offset ?? Vector2.zero), color, moveY, duration);
        }
        
        /// <summary>
        /// Pokaż popup w zadanej pozycji (układ rodzica canvasa – anchored).
        /// </summary>
        public void Show(string text, Vector2 anchoredPos, Color color, float moveY = 50f, float duration = 1f)
        {
            var popupBase = GetPopupBase();
            popupBase.Show(text, anchoredPos, color, moveY, duration);
        }
    }
}
