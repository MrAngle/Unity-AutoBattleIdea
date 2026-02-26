using MageFactory.Character.Api.Event;
using MageFactory.Character.Api.Event.Dto;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Shared.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Context.Combat {
    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler, IHpChangedEventListener,
        ICharacterDeathEventListener {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICombatCharacter _character;
        private IUiCombatContextEventPublisher uiCombatContextEventPublisher;
        private ICharacterEventRegistry characterEventRegistry;

        public static CharacterPrefabAggregate create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
                                                      ICombatCharacter characterData,
                                                      IUiCombatContextEventPublisher uiCombatContextEventPublisher,
                                                      ICharacterEventRegistry characterEventRegistry) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.setup(characterData, uiCombatContextEventPublisher, characterEventRegistry);


            return prefab;
        }

        private void setup(ICombatCharacter character,
                           IUiCombatContextEventPublisher paramUiCombatContextEventPublisher,
                           ICharacterEventRegistry paramCharacterEventRegistry) {
            _character = NullGuard.NotNullOrThrow(character);
            uiCombatContextEventPublisher = NullGuard.NotNullOrThrow(paramUiCombatContextEventPublisher);
            characterEventRegistry = NullGuard.NotNullOrThrow(paramCharacterEventRegistry);

            if (_character != null) {
                characterEventRegistry.subscribe((IHpChangedEventListener)this);
                characterEventRegistry.subscribe((ICharacterDeathEventListener)this);
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

        private void refreshUI() {
            if (_character == null) return;

            nameText.text = _character.getName();

            var ratio = 0f;
            if (_character.getMaxHp() > 0)
                ratio = (float)_character.getCurrentHp() / _character.getMaxHp();

            hpBarImage.fillAmount = ratio;
        }

        private void cleanup() {
            characterEventRegistry?.unsubscribe((IHpChangedEventListener)this);
            characterEventRegistry?.unsubscribe((ICharacterDeathEventListener)this);

            _character?.cleanup();
        }

        public void onEvent(in HpChangedDtoEvent ev) {
            if (ev.characterId != _character.getId()) {
                return;
            }

            PopupManager.Instance.ShowHpChangeDamage(this, ev.newHp - ev.previousHpValue);
            refreshUI();
        }

        public void onEvent(in CharacterDeathDtoEvent ev) {
            if (ev.characterId != _character.getId()) {
                return;
            }

            Destroy(gameObject);
        }
    }
}