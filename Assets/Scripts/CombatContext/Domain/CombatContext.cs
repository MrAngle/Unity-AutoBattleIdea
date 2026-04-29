using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContext.Domain.CombatCapabilities;
using MageFactory.CombatContext.Domain.CombatCapabilities.MageFactory.CombatContext.Domain;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Contract;
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
        private ICombatCapabilities combatCapabilities;

        private CombatContext(ICombatCharacterFactory characterFactory,
                              ICombatContextEventPublisher combatContextEventPublisher) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);
            NullGuard.NotNullOrThrow(characters);
        }

        private void initializeCapabilities() {
            if (combatCapabilities != null) {
                throw new InvalidOperationException("Combat capabilities already initialized.");
            }

            CombatCommandBus combatCommandBus = new CombatCommandBus(this);
            CombatQueries combatQueries = new CombatQueries(characters);

            combatCapabilities = new CombatCapabilities.CombatCapabilities(combatCommandBus, combatQueries);
        }

        internal static CombatContext create(ICombatCharacterFactory paramCharacterFactory,
                                             ICombatContextEventPublisher combatContextEventPublisher,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(paramCharacterFactory, combatContextEventPublisher);
            combatContext.initializeCapabilities();

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

        public ICombatCapabilities getCombatCapabilities() {
            return combatCapabilities;
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
        public void consumeFlow(ConsumeFlowCommand consumeFlowCommand) {
            if (consumeFlowCommand.flowOwner?.getFlowOwnerCharacterId() == null) {
                return;
            }

            ICombatCharacterFacade combatCombatCharacter =
                characters[consumeFlowCommand.flowOwner.getFlowOwnerCharacterId()];
            if (combatCombatCharacter == null) {
                return;
            }

            switch (consumeFlowCommand.flowKind) {
                case FlowKind.Damage:
                    processOffensiveFlow(consumeFlowCommand);
                    break;
                case FlowKind.Defense:
                    processDefensiveFlow(combatCombatCharacter, consumeFlowCommand);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(consumeFlowCommand.flowKind),
                        "Unsupported flow kind.");
            }
        }

        private DamageToDeal processDefensiveFlow(ICombatCharacterFacade combatCharacterFacade,
                                                  ConsumeFlowCommand consumeFlowCommand) {
            throw new NotImplementedException();
        }

        private void processOffensiveFlow(ConsumeFlowCommand consumeFlowCommand) {
            if (tryGetRandomEnemyOf(consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    out ICombatCharacterFacade enemy)) {
                // TODO: targeting should be in flow
                DamageIncomingCombatEvent damageIncomingCombatEvent = new DamageIncomingCombatEvent(
                    enemy.query().getCharacterInfo().getCharacterId(),
                    consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    DamageRole.ATTACK,
                    consumeFlowCommand.damageToDeal);

                dispatchCombatEvent(damageIncomingCombatEvent);
            }
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

        internal void createFlow(CreateFlowCombatCommand combatCommand) {
            NullGuard.NotNullOrThrow(combatCommand);

            if (!characters.TryGetValue(combatCommand.characterId, out ICombatCharacterFacade characterFacade)) {
                throw new InvalidOperationException(
                    $"Character with id '{combatCommand.characterId}' is not registered in CombatContext."
                );
            }

            characterFacade.command().createFlow(combatCommand.itemId, this, combatCapabilities);
        }

        private void dispatchCombatEvent(CombatEvent combatEvent) {
            if (combatEvent == null) {
                throw new ArgumentNullException(nameof(combatEvent));
            }

            if (!characters.TryGetValue(combatEvent.getTargetCharacterId(),
                    out ICombatCharacterFacade targetCharacter)) {
                throw new InvalidOperationException(
                    $"Target character with id '{combatEvent.getTargetCharacterId()}' is not registered in CombatContext.");
            }

            // W przyszłości:
            // 1. apply global modifiers
            // 2. update global combat statistics

            targetCharacter.command().consumeCombatEvent(combatEvent);
        }
    }
}