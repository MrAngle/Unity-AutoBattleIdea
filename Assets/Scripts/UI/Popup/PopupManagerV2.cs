using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popup
{
    public class PopupManagerV2 : MonoBehaviour
    {
        public static PopupManagerV2 Instance { get; private set; }

        [Header("Parent (root Canvas)")]
        [SerializeField] private Canvas parentCanvas;            // główny Canvas sceny
        [SerializeField] private RectTransform parentRect;       // jego RectTransform

        [Header("Prefab & Pool")]
        [SerializeField] private PopupBase popupPrefab;          // prefab pojedynczego popupu (bez Canvas!)
        [SerializeField] private int initialPoolSize = 10;

        private readonly Queue<PopupBase> _pool = new();
        private Camera _uiCam;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // Auto-wire jeśli nie ustawiono w Inspektorze
            if (!parentCanvas) parentCanvas = FindObjectOfType<Canvas>();
            if (!parentRect && parentCanvas) parentRect = (RectTransform)parentCanvas.transform;

            _uiCam = parentCanvas && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : (parentCanvas ? parentCanvas.worldCamera : null);

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

        /// <summary>
        /// Pokaż popup w zadanej pozycji (układ rodzica canvasa – anchored).
        /// </summary>
        public void Show(string text, Vector2 anchoredPos, Color color, float moveY = 50f, float duration = 1f)
        {
            var popupBase = GetPopupBase();
            popupBase.Show(text, anchoredPos, color, moveY, duration);
        }

        /// <summary>
        /// Pokaż popup na środku komponentu, który wywołuje (UI: środek Recta, inne: Transform.position).
        /// </summary>
        public void Show(Component owner, string textToShow, Color color, Vector2? offset = null, float moveY = 50f, float duration = 1f)
        {
            Transform t = owner.transform;

            Vector3 worldPos = t is RectTransform rt
                ? rt.TransformPoint(rt.rect.center) // środek elementu UI
                : t.position;                        // obiekt nie-UI

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(_uiCam, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screen, _uiCam, out var local);

            Show(textToShow, local + (offset ?? Vector2.zero), color, moveY, duration);
        }
        
        public void Show(Transform ownertransform, string textToShow, Color color, Vector2? offset = null, float moveY = 50f, float duration = 1f)
        {

            Vector3 worldPos = ownertransform is RectTransform rectTransform
                ? rectTransform.TransformPoint(rectTransform.rect.center)
                : ownertransform.position;

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(_uiCam, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screen, _uiCam, out var local);

            Show(textToShow, local + (offset ?? Vector2.zero), color, moveY, duration);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!parentCanvas) parentCanvas = FindObjectOfType<Canvas>();
            if (!parentRect && parentCanvas) parentRect = (RectTransform)parentCanvas.transform;
        }
#endif
    }
}
