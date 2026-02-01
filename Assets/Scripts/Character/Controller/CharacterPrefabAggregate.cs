using Context;
using Contracts.Character;
using Registry;
using Shared.Utility;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Controller.Character {
    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICharacter _character;

        private CharacterAggregateContext _characterAggregateContext;

        private void OnDisable() {
            Cleanup();
        }

        private void OnDestroy() {
            Cleanup();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (_character == null) return;

            Debug.Log($"Kliknięto postać: {_character.getName()}");

            _characterAggregateContext.SetCharacterAggregateContext(_character);

            // np. pokaż szczegóły, zaznacz postać itd.
            // OnClicked?.Invoke(this, _character);
        }

        [Inject]
        public void Construct(
            CharacterAggregateContext characterAggregateContext
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
        }

        public static CharacterPrefabAggregate Create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
            ICharacter characterData, CharacterAggregateContext characterAggregateContext) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.Setup(characterData, characterAggregateContext);

            return prefab;
        }

        private void Setup(ICharacter character, CharacterAggregateContext characterAggregateContext) {
            _character = NullGuard.NotNullOrThrow(character);
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);

            if (_character != null) {
                _character.OnHpChanged += HandleHpChanged;
                _character.OnDeath += OnDeath;
                CharacterRegistry.Instance.Register(_character);
            }

            RefreshUI();
        }

        private void HandleHpChanged(ICharacter ch, long newHp, long previousHpValue) {
            PopupManager.Instance.ShowHpChangeDamage(this, newHp - previousHpValue);
            RefreshUI();
        }

        private void OnDeath(ICharacter ch) {
            CharacterRegistry.Instance.Unregister(_character);
            Destroy(gameObject);
        }

        public void RefreshUI() {
            if (_character == null) return;

            nameText.text = _character.getName();

            var ratio = 0f;
            if (_character.getMaxHp() > 0)
                ratio = (float)_character.getCurrentHp() / _character.getMaxHp();

            hpBarImage.fillAmount = ratio;
        }

        private void Cleanup() {
            if (_character != null) {
                _character.OnHpChanged -= HandleHpChanged;
                _character.OnDeath -= OnDeath;
                _character.cleanup();
            }
        }
    }
}