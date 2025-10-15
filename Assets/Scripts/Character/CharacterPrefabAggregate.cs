using Registry;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // jeśli używasz TextMeshPro

namespace Character
{
    public class CharacterPrefabAggregate : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public Image hpBarImage;

        private CharacterAggregate _character;

        public static CharacterPrefabAggregate Create(CharacterPrefabAggregate slotPrefab, Transform slotParent, CharacterAggregate characterData)
        {
            CharacterPrefabAggregate prefab = Instantiate(slotPrefab, slotParent, false);
            prefab.Setup(characterData);
            
            return prefab;
        }
        
        private void Setup(CharacterAggregate character)
        {
            _character = character;

            if (_character != null)
            {
                _character.OnHpChanged += HandleHpChanged;
                _character.OnDeath += OnDeath;
                CharacterRegistry.Instance.Register(_character);
            }

            RefreshUI();
        }

        private void HandleHpChanged(CharacterAggregate ch, int newHp)
        {
            // tutaj odśwież UI
            RefreshUI();
        }
        
        private void OnDeath(CharacterAggregate ch)
        {
            CharacterRegistry.Instance.Unregister(_character);
            Destroy(gameObject);
        }

        public void RefreshUI()
        {
            if (_character == null) return;

            nameText.text = _character.Name;

            float ratio = 0f;
            if (_character.MaxHp > 0)
                ratio = (float)_character.CurrentHp / _character.MaxHp;

            hpBarImage.fillAmount = ratio;
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            Cleanup();
        }
        
        private void Cleanup()
        {
            if (_character != null)
            {
                _character.OnHpChanged -= HandleHpChanged;
                _character.OnDeath -= OnDeath;
                _character.Cleanup();
            }
        }
        
    }
}