using MageFactory.Shared.Model;

namespace MageFactory.Character.Api.Dto {
    public readonly struct CharacterCreateCommand {
        public string Name { get; }
        public int MaxHp { get; }

        public Team Team { get; }

        public CharacterCreateCommand(string name, int maxHp, Team team) {
            Name = name;
            MaxHp = maxHp;
            Team = team;
        }
    }
}