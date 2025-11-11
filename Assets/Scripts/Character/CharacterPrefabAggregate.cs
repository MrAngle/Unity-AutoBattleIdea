using System;
using Registry;
using TMPro;
using UI.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace Character {

    public class CharacterPrefabAggregate : MonoBehaviour {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private ICharacterAggregateFacade _character;

        private void OnDisable() {
            Cleanup();
        }

        private void OnDestroy() {
            Cleanup();
        }

        public static CharacterPrefabAggregate Create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
            ICharacterAggregateFacade characterData) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.Setup(characterData);

            return prefab;
        }

        private void Setup(ICharacterAggregateFacade character) {
            _character = character;

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

        private void Cleanup() {
            if (_character != null) {
                _character.OnHpChanged -= HandleHpChanged;
                _character.OnDeath -= OnDeath;
                _character.Cleanup();
            }
        }
    }
}