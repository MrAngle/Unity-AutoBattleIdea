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
    internal class CombatContext : ICombatContext, IFlowConsumer {
        private readonly Dictionary<Id<CharacterId>, ICombatCharacterFacade> characters = new();

        private readonly ICombatCharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;
        private readonly Random random = new();

        private CombatContext(ICombatCharacterFactory characterFactory,
                              ICombatContextEventPublisher combatContextEventPublisher) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);
            NullGuard.NotNullOrThrow(characters);
        }

        internal static CombatContext create(ICombatCharacterFactory paramCharacterFactory,
                                             ICombatContextEventPublisher combatContextEventPublisher,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(paramCharacterFactory, combatContextEventPublisher);

            foreach (CreateCombatCharacterCommand createCombatCharacterCommand in charactersToCreate) {
                combatContext.registerCharacter(createCombatCharacterCommand);
            }

            combatContextEventPublisher.publish(new CombatContextCreatedDtoEvent(combatContext));
            return combatContext;
        }

        public ICombatCharacterFacade getRandomCharacter() {
            return characters.Values.FirstOrDefault();
        }

        public IReadOnlyCollection<ICombatCharacterFacade> getAllCharacters() {
            return characters.Values;
        }

        public ICombatCharacterFacade getCombatCharacterById(Id<CharacterId> id) {
            return characters[id];
        }

        public IFlowConsumer getFlowConsumer() {
            return this;
        }

        private void registerCharacter(CreateCombatCharacterCommand createCombatCharacterCommand) {
            ICombatCharacterFacade combatCombatCharacter = characterFactory.create(createCombatCharacterCommand);

            Id<CharacterId> characterId = combatCombatCharacter.query().getCharacterInfo().getCharacterId();
            if (characters.ContainsKey(characterId)) {
                throw new InvalidOperationException(
                    $"Character with id '{characterId}' is already registered in CombatContext.");
            }

            characters.Add(characterId, combatCombatCharacter);
            combatContextEventPublisher.publish(new CombatCharacterCreatedDtoEvent());
        }


        // TODO: return flow result
        public DamageToDeal consumeFlow(ConsumeFlowCommand consumeFlowCommand) {
            if (consumeFlowCommand.flowOwner?.getFlowOwnerCharacterId() == null) {
                return DamageToDeal.NO_POWER;
            }

            ICombatCharacterFacade combatCombatCharacter =
                characters[consumeFlowCommand.flowOwner.getFlowOwnerCharacterId()];
            if (combatCombatCharacter == null) {
                return DamageToDeal.NO_POWER;
            }

            return consumeFlowCommand.flowKind switch {
                FlowKind.Damage => processOffensiveFlow(consumeFlowCommand),
                FlowKind.Defense => processDefensiveFlow(combatCombatCharacter, consumeFlowCommand),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private DamageToDeal processDefensiveFlow(ICombatCharacterFacade combatCharacterFacade,
                                                  ConsumeFlowCommand consumeFlowCommand) {
            throw new NotImplementedException();
        }

        private DamageToDeal processOffensiveFlow(ConsumeFlowCommand consumeFlowCommand) {
            // target selection should be specified in the flow command
            if (tryGetRandomEnemyOf(consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    out ICombatCharacterFacade enemy)) {
                enemy.command()
                    .takeDamage(DamageToReceive.fromPowerAmount(consumeFlowCommand.damageToDeal));
                return consumeFlowCommand.damageToDeal;
            }

            return DamageToDeal.NO_POWER;
        }

        public bool tryGetRandomEnemyOf(Id<CharacterId> sourceId, out ICombatCharacterFacade enemy) {
            enemy = null;
            if (!characters.TryGetValue(sourceId, out ICombatCharacterFacade sourceCharacter)) {
                Debug.Log($"[CombatContext] sourceId {sourceId} not found");
                return false;
            }

            Team sourceTeam = sourceCharacter.query().getCharacterInfo().getTeam();
            var enemies = characters.Values
                .Where(c => !Equals(c.query().getCharacterInfo().getCharacterId(), sourceId))
                .Where(c => c.query().getCharacterInfo().getTeam() != sourceTeam)
                // todo: filter out dead characters
                .ToList();

            if (enemies.Count == 0)
                return false;

            int index = random.Next(enemies.Count);
            enemy = enemies[index];
            Debug.Log(
                $"[CombatContext] Picked enemy: {enemy.query().getCharacterInfo().getCharacterName()}({enemy.query().getCharacterInfo().getTeam()})");
            return true;
        }
    }
}