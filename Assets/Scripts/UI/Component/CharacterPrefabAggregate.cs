using MageFactory.Character.Api.Event.Dto;
using MageFactory.CombatContext.Contract;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat.Event;
using MageFactory.UI.Shared.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageFactory.UI.Component {
    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICombatCharacterFacade combatCharacter;
        private IUiCombatContextEventPublisher uiCombatContextEventPublisher;

        public static CharacterPrefabAggregate create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
                                                      ICombatCharacterFacade combatCharacterData,
                                                      IUiCombatContextEventPublisher uiCombatContextEventPublisher) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.setup(combatCharacterData, uiCombatContextEventPublisher);


            return prefab;
        }

        private void setup(ICombatCharacterFacade combatCharacter,
                           IUiCombatContextEventPublisher paramUiCombatContextEventPublisher) {
            this.combatCharacter = NullGuard.NotNullOrThrow(combatCharacter);
            uiCombatContextEventPublisher = NullGuard.NotNullOrThrow(paramUiCombatContextEventPublisher);

            refreshUI();
        }

        public void hpChange(in CharacterHpChangedDtoEvent ev) {
            PopupManager.Instance.ShowHpChangeDamage(this, ev.newHp - ev.previousHpValue);
            refreshUI();
        }

        public void guardAbsorbedDamage(in CharacterGuardAbsorbedDamageDtoEvent ev) {
            string text = ev.remainingDamage > 0
                ? $"G -{ev.blockedDamage} / {ev.remainingDamage}"
                : $"G -{ev.blockedDamage} / 0";
            PopupManager.Instance.Show(
                this,
                text,
                new Color(0.38f, 0.95f, 0.82f, 1f),
                new Vector2(0f, 28f),
                70f,
                1.15f);
            refreshUI();
        }

        public void destroy(in CharacterDeathDtoEvent ev) {
            Destroy(gameObject);
        }

        public Vector3 getCenterWorldPosition() {
            if (transform is RectTransform rectTransform) {
                return rectTransform.TransformPoint(rectTransform.rect.center);
            }

            return transform.position;
        }

        private void OnDisable() {
            cleanup();
        }

        private void OnDestroy() {
            cleanup();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (combatCharacter == null) return;

            Debug.Log($"Kliknięto postać: {combatCharacter.query().getCharacterInfo().getCharacterName()}");

            uiCombatContextEventPublisher.publish(
                new UiCombatCharacterSelectedEvent(combatCharacter.query().getCharacterInfo().getCharacterId()));
        }

        private void refreshUI() {
            if (combatCharacter == null) return;

            nameText.text = combatCharacter.query().getCharacterInfo().getCharacterName();
            long characterMaxHp = combatCharacter.query().getCharacterInfo().getMaxHp();
            long characterCurrentHp = combatCharacter.query().getCharacterInfo().getCurrentHp();

            var ratio = 0f;
            if (characterMaxHp > 0) {
                ratio = (float)characterCurrentHp / characterMaxHp;
            }

            hpBarImage.fillAmount = ratio;
        }

        private void cleanup() {
            combatCharacter?.command().cleanup();
        }
    }
}