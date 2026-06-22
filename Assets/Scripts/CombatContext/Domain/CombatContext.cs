using System;
using System.Collections.Generic;
using System.Linq;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.CombatContext.Domain.CombatCapabilities;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Flow.Contract;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using UnityEngine;
using CombatCapabilitiesContainer = MageFactory.CombatContext.Domain.CombatCapabilities.CombatCapabilities;
using Random = System.Random;

namespace MageFactory.CombatContext.Domain {
    internal class CombatContext : ICombatContext, IFlowConsumer {
        private readonly Dictionary<Id<CharacterId>, ICombatCharacterFacade> characters = new();
        private readonly Dictionary<CombatEventType, int> combatEventCountsByType = new();
        private readonly List<ICombatCharacterFacade> enemyCandidates = new();

        private readonly ICombatCharacterFactory characterFactory;
        private readonly ICombatContextEventPublisher combatContextEventPublisher;
        private readonly CombatRuntimeSettings combatRuntimeSettings;
        private readonly Random random = new();
        private ICombatCapabilities combatCapabilities;

        private CombatContext(ICombatCharacterFactory characterFactory,
                              ICombatContextEventPublisher combatContextEventPublisher,
                              CombatRuntimeSettings combatRuntimeSettings) {
            this.characterFactory = NullGuard.NotNullOrThrow(characterFactory);
            this.combatContextEventPublisher = NullGuard.NotNullOrThrow(combatContextEventPublisher);
            this.combatRuntimeSettings = NullGuard.NotNullOrThrow(combatRuntimeSettings);
            NullGuard.NotNullOrThrow(characters);
        }

        private void initializeCapabilities() {
            if (combatCapabilities != null) {
                throw new InvalidOperationException("Combat capabilities already initialized.");
            }

            CombatCommandBus combatCommandBus = new CombatCommandBus(this);
            CombatQueries combatQueries = new CombatQueries(characters, combatEventCountsByType);

            combatCapabilities = new CombatCapabilitiesContainer(combatCommandBus, combatQueries);
        }

        internal static CombatContext create(ICombatCharacterFactory paramCharacterFactory,
                                             ICombatContextEventPublisher combatContextEventPublisher,
                                             CombatRuntimeSettings combatRuntimeSettings,
                                             IReadOnlyList<CreateCombatCharacterCommand> charactersToCreate) {
            CombatContext combatContext = new CombatContext(
                paramCharacterFactory,
                combatContextEventPublisher,
                combatRuntimeSettings);
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

            if (!characters.TryGetValue(
                    consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    out ICombatCharacterFacade combatCombatCharacter)) {
                return;
            }

            publishFlowCompletionFact(combatCombatCharacter, consumeFlowCommand);
            processGuardOutput(combatCombatCharacter, consumeFlowCommand);

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

        public void discardFlow(DiscardFlowCommand discardFlowCommand) {
            if (discardFlowCommand.flowOwner?.getFlowOwnerCharacterId() == null) {
                return;
            }

            if (!characters.TryGetValue(
                    discardFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    out ICombatCharacterFacade combatCombatCharacter)) {
                return;
            }

            Id<CharacterId> characterId = combatCombatCharacter.query().getCharacterInfo().getCharacterId();
            combatContextEventPublisher.publish(new FlowNoOutputDtoEvent(
                characterId,
                discardFlowCommand.attackPower.getPower(),
                discardFlowCommand.guardPower.getPower(),
                discardFlowCommand.finalProcessingSlot,
                false));

            if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                Debug.Log(
                    $"[CombatContext] Port-aware flow discarded without output for character={characterId}, " +
                    $"attackPower={discardFlowCommand.attackPower.getPower()}, guardPower={discardFlowCommand.guardPower.getPower()}");
            }
        }

        private void publishFlowCompletionFact(
            ICombatCharacterFacade combatCharacterFacade,
            ConsumeFlowCommand consumeFlowCommand) {
            Id<CharacterId> characterId = combatCharacterFacade.query().getCharacterInfo().getCharacterId();

            if (consumeFlowCommand.reachedOutputPort) {
                combatContextEventPublisher.publish(new FlowOutputReachedDtoEvent(
                    characterId,
                    consumeFlowCommand.attackPower.getPower(),
                    consumeFlowCommand.guardPower.getPower(),
                    consumeFlowCommand.finalProcessingSlot));
                return;
            }

            if (!combatRuntimeSettings.shouldPublishLegacyFlowNoOutputWarnings()) {
                return;
            }

            combatContextEventPublisher.publish(new FlowNoOutputDtoEvent(
                characterId,
                consumeFlowCommand.attackPower.getPower(),
                consumeFlowCommand.guardPower.getPower(),
                consumeFlowCommand.finalProcessingSlot,
                true));
        }

        private void processGuardOutput(ICombatCharacterFacade combatCharacterFacade,
                                        ConsumeFlowCommand consumeFlowCommand) {
            if (!consumeFlowCommand.hasGuardPower()) {
                return;
            }

            if (!combatCharacterFacade.command()
                    .tryAddGuardPower(consumeFlowCommand.guardPower, out PreparedGuardAddResult guardAddResult)) {
                return;
            }

            PreparedGuardState guardState = guardAddResult.getAddedGuardState();
            bool replacedGuard = guardAddResult.hasReplacedGuard();
            PreparedGuardState replacedGuardState = replacedGuard
                ? guardAddResult.getReplacedGuardState()
                : default;

            combatContextEventPublisher.publish(new FlowGuardCreatedDtoEvent(
                combatCharacterFacade.query().getCharacterInfo().getCharacterId(),
                guardState.getGuardId(),
                guardState.getGuardPower().getPower(),
                consumeFlowCommand.finalProcessingSlot,
                replacedGuard,
                replacedGuard ? replacedGuardState.getGuardId() : default,
                replacedGuard ? replacedGuardState.getGuardPower().getPower() : 0));
        }

        private void processDefensiveFlow(ICombatCharacterFacade combatCharacterFacade,
                                          ConsumeFlowCommand consumeFlowCommand) {
            ResolvedDamage resolvedDamage = ResolvedDamage.fromPowerAmount(consumeFlowCommand.attackPower);
            DamageTaken damageTaken = combatCharacterFacade.command().applyResolvedDamage(resolvedDamage);

            if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                Debug.Log(
                    $"[CombatContext] Defensive flow resolved for character={combatCharacterFacade.query().getCharacterInfo().getCharacterId()}, " +
                    $"resolvedDamage={resolvedDamage.getPower()}, damageTaken={damageTaken.getPower()}");
            }
        }

        private void processOffensiveFlow(ConsumeFlowCommand consumeFlowCommand) {
            if (!consumeFlowCommand.hasAttackPower()) {
                return;
            }

            if (tryGetRandomEnemyOf(consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    out ICombatCharacterFacade enemy)) {
                // TODO: targeting should be in flow
                combatContextEventPublisher.publish(new FlowAttackCreatedDtoEvent(
                    consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    enemy.query().getCharacterInfo().getCharacterId(),
                    consumeFlowCommand.attackPower.getPower(),
                    consumeFlowCommand.finalProcessingSlot));

                IncomingAttackDamageCombatEvent damageIncomingCombatEvent = new IncomingAttackDamageCombatEvent(
                    enemy.query().getCharacterInfo().getCharacterId(),
                    consumeFlowCommand.flowOwner.getFlowOwnerCharacterId(),
                    DamageToDeal.fromPowerAmount(consumeFlowCommand.attackPower));

                dispatchCombatEvent(damageIncomingCombatEvent);
            }
        }

        public bool tryGetRandomEnemyOf(Id<CharacterId> sourceId, out ICombatCharacterFacade enemy) {
            enemy = null;
            if (!characters.TryGetValue(sourceId, out ICombatCharacterFacade sourceCharacter)) {
                if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                    Debug.Log($"[CombatContext] sourceId {sourceId} not found");
                }

                return false;
            }

            Team sourceTeam = sourceCharacter.query().getCharacterInfo().getTeam();
            enemyCandidates.Clear();

            foreach (ICombatCharacterFacade character in characters.Values) {
                IReadOnlyCombatCharacterData characterInfo = character.query().getCharacterInfo();

                if (Equals(characterInfo.getCharacterId(), sourceId)) {
                    continue;
                }

                if (characterInfo.getTeam() == sourceTeam) {
                    continue;
                }

                if (characterInfo.getCurrentHp() <= 0) {
                    continue;
                }

                enemyCandidates.Add(character);
            }

            if (enemyCandidates.Count == 0) {
                return false;
            }

            int index = random.Next(enemyCandidates.Count);
            enemy = enemyCandidates[index];
            if (combatRuntimeSettings.shouldLogCombatHotPath()) {
                Debug.Log(
                    $"[CombatContext] Picked enemy: {enemy.query().getCharacterInfo().getCharacterName()}({enemy.query().getCharacterInfo().getTeam()})");
            }

            return true;
        }

        internal bool createFlow(CreateFlowCombatCommand combatCommand) {
            NullGuard.NotNullOrThrow(combatCommand);

            if (!characters.TryGetValue(combatCommand.characterId, out ICombatCharacterFacade characterFacade)) {
                throw new InvalidOperationException(
                    $"Character with id '{combatCommand.characterId}' is not registered in CombatContext."
                );
            }

            bool created = characterFacade.command().createFlow(combatCommand.itemId, this, combatCapabilities);
            if (created) {
                combatContextEventPublisher.publish(new FlowInputStartedDtoEvent(
                    combatCommand.characterId,
                    combatCommand.itemId));
            }

            return created;
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

            recordCombatEvent(combatEvent);

            targetCharacter.command().consumeCombatEvent(combatEvent, this, combatCapabilities);
        }

        private void recordCombatEvent(CombatEvent combatEvent) {
            CombatEvent eventToRecord = NullGuard.NotNullOrThrow(combatEvent);
            CombatEventType eventType = eventToRecord.getType();

            combatEventCountsByType.TryGetValue(eventType, out int currentCount);
            combatEventCountsByType[eventType] = currentCount + 1;
        }
    }
}