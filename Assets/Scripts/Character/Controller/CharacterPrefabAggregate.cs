using MageFactory.Character.Api;
using MageFactory.Context;
using MageFactory.Registry;
using MageFactory.Shared.Utility;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace MageFactory.Character.Controller {
    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICharacter _character;

        private CharacterAggregateContext _characterAggregateContext;

        [Inject]
        public void construct(
            CharacterAggregateContext characterAggregateContext
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
        }


        private void OnDisable() {
            cleanup();
        }

        private void OnDestroy() {
            cleanup();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (_character == null) return;

            Debug.Log($"Kliknięto postać: {_character.getName()}");

            _characterAggregateContext.setCharacterAggregateContext(_character);
        }

        public static CharacterPrefabAggregate create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
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

        private void RefreshUI() {
            if (_character == null) return;

            nameText.text = _character.getName();

            var ratio = 0f;
            if (_character.getMaxHp() > 0)
                ratio = (float)_character.getCurrentHp() / _character.getMaxHp();

            hpBarImage.fillAmount = ratio;
        }

        private void cleanup() {
            if (_character == null) {
                return;
            }

            _character.OnHpChanged -= HandleHpChanged;
            _character.OnDeath -= OnDeath;
            _character.cleanup();
        }
    }
}