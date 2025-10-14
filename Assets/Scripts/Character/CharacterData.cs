namespace Character
{
    public class CharacterData
    {
        public string Name;
        public int MaxHp;
        public int CurrentHp;

        public CharacterData(string name, int maxHP)
        {
            Name = name;
            MaxHp = maxHP;
            CurrentHp = maxHP;
        }

        public void TakeDamage(int dmg)
        {
            CurrentHp -= dmg;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        public void Heal(int amount)
        {
            CurrentHp += amount;
            if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}