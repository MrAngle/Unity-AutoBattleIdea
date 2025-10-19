using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popup {
    public enum PopupType { Damage, Heal, Buff, Critical }

    // [RequireComponent(typeof(RectTransform))]
    // public class PopupBase : MonoBehaviour
    // {
    //     [SerializeField] private TextMeshProUGUI _text;
    //     [SerializeField] private CanvasGroup _canvasGroup;
    //
    //     private RectTransform _rt;
    //     private Sequence _seq;
    //
    //     private void Awake()
    //     {
    //         _rt = (RectTransform)transform;
    //     }
    //
    //     private void OnDisable()
    //     {
    //         _seq?.Kill();
    //         _seq = null;
    //     }
    //
    //     /// <summary>
    //     /// Ustaw parametry i odpal animację.
    //     /// anchoredPos – pozycja w układzie rodzica (anchored).
    //     /// </summary>
    //     public void Show(string text, Vector2 anchoredPos, PopupType type,
    //         Color color, float moveY = 50f, float duration = 0.8f)
    //     {
    //         // reset
    //         _seq?.Kill();
    //         gameObject.SetActive(true);
    //
    //         _rt.anchoredPosition = anchoredPos;
    //         _rt.localScale = Vector3.one * 0.8f;
    //
    //         _text.text = text;
    //         _text.color = color;
    //         _canvasGroup.alpha = 1f;
    //
    //         // animacja
    //         _seq = DOTween.Sequence();
    //         _seq.Append(_rt.DOAnchorPosY(anchoredPos.y + moveY, duration).SetEase(Ease.OutCubic));
    //         _seq.Join(_rt.DOScale(Vector3.one * 1.2f, duration * 0.4f).SetEase(Ease.OutBack));
    //         _seq.Append(_rt.DOScale(Vector3.one, duration * 0.6f).SetEase(Ease.InOutQuad));
    //         _seq.Insert(duration * 0.3f, _canvasGroup.DOFade(0f, duration * 0.7f));
    //
    //         _seq.OnComplete(() =>
    //         {
    //             // ważne: ubijamy sekwencję, zanim wróci do puli
    //             _seq.Kill();
    //             _seq = null;
    //             PopupManager.Instance.ReturnToPool(this);
    //         });
    //     }
    // }
    
    
    
    [RequireComponent(typeof(RectTransform))]
    public class PopupBase : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _group;

        private RectTransform _rt;
        private Sequence _seq;

        private void Awake()
        {
            _rt = (RectTransform)transform;
            if (_group == null) _group = GetComponent<CanvasGroup>();
            // popup nie powinien blokować kliknięć:
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }

        private void OnDisable() { _seq?.Kill(); _seq = null; }

        // anchoredPos – pozycja w układzie rodzica (RectTransform parenta)
        public void Show(string text, Vector2 anchoredPos, Color color, float moveY = 50f, float duration = 1f)
        {
            _seq?.Kill();
            gameObject.SetActive(true);

            _rt.anchoredPosition = anchoredPos;
            _rt.localScale = Vector3.one * 0.8f;

            _text.text = text;
            _text.color = color;
            _group.alpha = 1f;

            _seq = DOTween.Sequence();
            _seq.Append(_rt.DOAnchorPosY(anchoredPos.y + moveY, duration).SetEase(Ease.OutCubic));
            _seq.Join(_rt.DOScale(Vector3.one * 1.15f, duration * 0.35f).SetEase(Ease.OutBack));
            _seq.Insert(duration * 0.3f, _group.DOFade(0f, duration * 0.7f));
            _seq.OnComplete(() =>
            {
                _seq.Kill(); _seq = null;
                PopupManagerV2.Instance.ReturnToPool(this);
            });
        }
    }
}