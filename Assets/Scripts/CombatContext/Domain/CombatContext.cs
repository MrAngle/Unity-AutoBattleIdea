using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using Random = System.Random;

namespace MageFactory.CombatContext.Domain {
    internal class CombatContext : ICombatContext, IFlowConsumer, IReadCombatContext {
        private readonly Dictionary<Id<CharacterId>, ICombatCharacter> characters = new();

        private readonly ICharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;
        private readonly Random random = new();

        private CombatContext(ICharacterFactory characterFactory,
                              ICombatContextEventPublisher combatContextEventPublisher) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);
            NullGuard.NotNullOrThrow(characters);
        }

        internal static CombatContext create(ICharacterFactory paramCharacterFactory,
                                             ICombatContextEventPublisher combatContextEventPublisher,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(paramCharacterFactory, combatContextEventPublisher);

            foreach (CreateCombatCharacterCommand createCombatCharacterCommand in charactersToCreate) {
                combatContext.registerCharacter(createCombatCharacterCommand);
            }

            combatContextEventPublisher.publish(new CombatContextCreatedDtoEvent(combatContext));
            return combatContext;
        }

        public ICombatCharacter getRandomCharacter() {
            return characters.Values.FirstOrDefault();
        }

        public IReadOnlyCollection<ICombatCharacter> getAllCharacters() {
            return characters.Values;
        }

        public ICombatCharacter getCombatCharacterById(Id<CharacterId> id) {
            return characters[id];
        }

        public IFlowConsumer getFlowConsumer() {
            return this;
        }

        private void registerCharacter(CreateCombatCharacterCommand createCombatCharacterCommand) {
            ICombatCharacter combatCharacter = characterFactory.create(createCombatCharacterCommand);

            Id<CharacterId> characterId = combatCharacter.getId();
            if (characters.ContainsKey(characterId)) {
                throw new InvalidOperationException(
                    $"Character with id '{characterId}' is already registered in CombatContext.");
            }

            characters.Add(combatCharacter.getId(), combatCharacter);
            combatContextEventPublisher.publish(new CombatCharacterCreatedDtoEvent());
        }

        public DamageToDeal consumeFlow(ProcessFlowCommand flowCommand) {
            if (flowCommand.flowOwner?.getFlowOwnerCharacterId() == null) {
                return DamageToDeal.NO_POWER;
            }

            ICombatCharacter combatCharacter = characters[flowCommand.flowOwner.getFlowOwnerCharacterId()];
            if (combatCharacter == null) {
                return DamageToDeal.NO_POWER;
            }

            return combatCharacter.getCharacterCombatCapabilities().command().consumeFlow(flowCommand, this);
        }

        public bool tryGetRandomEnemyOf(Id<CharacterId> sourceId, out ICombatCharacter enemy) {
            enemy = null;
            if (!characters.TryGetValue(sourceId, out var source)) {
                Debug.Log($"[CombatContext] sourceId {sourceId} not found");
                return false;
            }

            Team sourceTeam = source.getTeam();

            var enemies = characters.Values
                .Where(c => !Equals(c.getId(), sourceId))
                .Where(c => c.getTeam() != sourceTeam)
                // todo: filter out dead characters
                .ToList();

            if (enemies.Count == 0)
                return false;

            int index = random.Next(enemies.Count);
            enemy = enemies[index];
            Debug.Log($"[CombatContext] Picked enemy: {enemy.getName()}({enemy.getTeam()})");
            return true;
        }
    }
}