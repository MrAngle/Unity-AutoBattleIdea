using UnityEngine;
using UnityEngine.UI;
using TMPro;  // jeśli używasz TextMeshPro

namespace UI
{
    public class CharacterSlotUI : MonoBehaviour
    {
        public TextMeshProUGUI nameText;     // przeciągnij komponent TMP w Inspectorze
        public Image hpBarImage;             // przeciągnij Image (typu Filled)

        private Character.CharacterData _character;

        public void SetCharacter(Character.CharacterData character)
        {
            _character = character;
            RefreshUI();
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

        // opcjonalnie: metoda, którą można wywołać, gdy postać dostaje obrażenia
        public void TakeDamage(int dmg)
        {
            if (_character == null) return;
            _character.TakeDamage(dmg);
            RefreshUI();
        }

        public void Heal(int amount)
        {
            if (_character == null) return;
            _character.Heal(amount);
            RefreshUI();
        }
    }
}