using System;
using Context;
using Registry;
using Shared.Utility;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Character {

    public class CharacterPrefabAggregate : MonoBehaviour, IPointerClickHandler  {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICharacterAggregateFacade _character;
        
        private CharacterAggregateContext _characterAggregateContext;
        
        [Inject]
        public void Construct(
            CharacterAggregateContext characterAggregateContext
        ) {
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);
        }
        

        private void OnDisable() {
            Cleanup();
        }

        private void OnDestroy() {
            Cleanup();
        }

        public static CharacterPrefabAggregate Create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
            ICharacterAggregateFacade characterData, CharacterAggregateContext characterAggregateContext) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.Setup(characterData, characterAggregateContext);

            return prefab;
        }

        private void Setup(ICharacterAggregateFacade character, CharacterAggregateContext characterAggregateContext) {
            _character = NullGuard.NotNullOrThrow(character);
            _characterAggregateContext = NullGuard.NotNullOrThrow(characterAggregateContext);

            if (_character != null) {
                _character.OnHpChanged += HandleHpChanged;
                _character.OnDeath += OnDeath;
                CharacterRegistry.Instance.Register(_character);
            }

            RefreshUI();
        }

        private void HandleHpChanged(CharacterAggregate ch, long newHp, long previousHpValue) {
            PopupManager.Instance.ShowHpChangeDamage(this, newHp - previousHpValue);
            RefreshUI();
        }

        private void OnDeath(ICharacterAggregateFacade ch) {
            CharacterRegistry.Instance.Unregister(_character);
            Destroy(gameObject);
        }

        public void RefreshUI() {
            if (_character == null) return;

            nameText.text = _character.GetName();

            var ratio = 0f;
            if (_character.GetMaxHp() > 0)
                ratio = (float)_character.GetCurrentHp() / _character.GetMaxHp();

            hpBarImage.fillAmount = ratio;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            if (_character == null) return;

            Debug.Log($"Kliknięto postać: {_character.GetName()}");
            
            _characterAggregateContext.SetCharacterAggregateContext(_character);

            // np. pokaż szczegóły, zaznacz postać itd.
            // OnClicked?.Invoke(this, _character);
        }

        private void Cleanup() {
            if (_character != null) {
                _character.OnHpChanged -= HandleHpChanged;
                _character.OnDeath -= OnDeath;
                _character.Cleanup();
            }
        }
    }
}