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

        private CharacterAggregate _character;

        private void OnDisable() {
            Cleanup();
        }

        private void OnDestroy() {
            Cleanup();
        }

        public static CharacterPrefabAggregate Create(CharacterPrefabAggregate slotPrefab, Transform slotParent,
            CharacterAggregate characterData) {
            var prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.Setup(characterData);

            return prefab;
        }

        private void Setup(CharacterAggregate character) {
            _character = character;

            if (_character != null) {
                _character.OnHpChanged += HandleHpChanged;
                _character.OnDeath += OnDeath;
                CharacterRegistry.Instance.Register(_character);
            }

            RefreshUI();
        }

        private void HandleHpChanged(CharacterAggregate ch, int newHp, int previousHpValue) {
            PopupManager.Instance.ShowHPChangeDamage(this, newHp - previousHpValue);
            RefreshUI();
        }

        private void OnDeath(CharacterAggregate ch) {
            CharacterRegistry.Instance.Unregister(_character);
            Destroy(gameObject);
        }

        public void RefreshUI() {
            if (_character == null) return;

            nameText.text = _character.Name;

            var ratio = 0f;
            if (_character.MaxHp > 0)
                ratio = (float)_character.CurrentHp / _character.MaxHp;

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