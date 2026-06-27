using MageFactory.Shared.Event;
using MageFactory.Shared.Id;

namespace MageFactory.Character.Api.Event.Dto {
    public readonly struct CharacterHpChangedDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long newHp;
        public readonly long previousHpValue;

        public CharacterHpChangedDtoEvent(Id<CharacterId> characterId,
                                          long newHp,
                                          long previousHpValue) {
            this.characterId = characterId;
            this.newHp = newHp;
            this.previousHpValue = previousHpValue;
        }
    }

    public readonly struct CharacterDeathDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;

        public CharacterDeathDtoEvent(Id<CharacterId> characterId) {
            this.characterId = characterId;
        }
    }

    public readonly struct CharacterGuardAbsorbedDamageDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long incomingDamage;
        public readonly long blockedDamage;
        public readonly long remainingDamage;
        public readonly int destroyedGuardCount;
        public readonly long remainingGuardPower;
        public readonly bool hasAffectedGuard;
        public readonly Id<GuardId> firstAffectedGuardId;

        public CharacterGuardAbsorbedDamageDtoEvent(
            Id<CharacterId> characterId,
            long incomingDamage,
            long blockedDamage,
            long remainingDamage,
            int destroyedGuardCount,
            long remainingGuardPower,
            bool hasAffectedGuard,
            Id<GuardId> firstAffectedGuardId) {
            this.characterId = characterId;
            this.incomingDamage = incomingDamage;
            this.blockedDamage = blockedDamage;
            this.remainingDamage = remainingDamage;
            this.destroyedGuardCount = destroyedGuardCount;
            this.remainingGuardPower = remainingGuardPower;
            this.hasAffectedGuard = hasAffectedGuard;
            this.firstAffectedGuardId = firstAffectedGuardId;
        }
    }

    public readonly struct CharacterStabilityAbsorbedDamageDtoEvent : IDomainEvent {
        public readonly Id<CharacterId> characterId;
        public readonly long incomingDamage;
        public readonly long reducedDamage;
        public readonly long remainingDamage;
        public readonly long stabilityStrain;
        public readonly long stabilityBefore;
        public readonly long stabilityAfter;
        public readonly long baselineStability;

        public CharacterStabilityAbsorbedDamageDtoEvent(
            Id<CharacterId> characterId,
            long incomingDamage,
            long reducedDamage,
            long remainingDamage,
            long stabilityStrain,
            long stabilityBefore,
            long stabilityAfter,
            long baselineStability) {
            this.characterId = characterId;
            this.incomingDamage = incomingDamage;
            this.reducedDamage = reducedDamage;
            this.remainingDamage = remainingDamage;
            this.stabilityStrain = stabilityStrain;
            this.stabilityBefore = stabilityBefore;
            this.stabilityAfter = stabilityAfter;
            this.baselineStability = baselineStability;
        }
    }
}