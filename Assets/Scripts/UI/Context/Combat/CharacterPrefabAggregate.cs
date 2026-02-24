using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Shared.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Context.Combat {
    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICombatCharacter _character;
        private IUiCombatContextEventPublisher uiCombatContextEventPublisher;

        public static CharacterPrefabAggregate create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
                                                      ICombatCharacter characterData,
                                                      IUiCombatContextEventPublisher uiCombatContextEventPublisher) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.setup(characterData, uiCombatContextEventPublisher);

            return prefab;
        }

        private void setup(ICombatCharacter character,
                           IUiCombatContextEventPublisher paramUiCombatContextEventPublisher) {
            _character = NullGuard.NotNullOrThrow(character);
            uiCombatContextEventPublisher = NullGuard.NotNullOrThrow(paramUiCombatContextEventPublisher);

            if (_character != null) {
                _character.OnHpChanged += handleHpChanged;
                _character.OnDeath += OnDeath;
            }

            refreshUI();
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

            uiCombatContextEventPublisher.publish(new UiCombatCharacterSelectedEvent(_character.getId()));
        }

        private void handleHpChanged(ICombatCharacter ch, long newHp, long previousHpValue) {
            PopupManager.Instance.ShowHpChangeDamage(this, newHp - previousHpValue);
            refreshUI();
        }

        private void OnDeath(ICombatCharacter ch) {
            Destroy(gameObject);
        }

        private void refreshUI() {
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

            _character.OnHpChanged -= handleHpChanged;
            _character.OnDeath -= OnDeath;
            _character.cleanup();
        }
    }
}