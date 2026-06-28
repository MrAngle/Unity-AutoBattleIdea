using MageFactory.CombatContext.Api.Event;
using MageFactory.CombatContext.Contract;
using MageFactory.CombatContextRuntime;
using MageFactory.CombatEvents;
using MageFactory.Shared.Id;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.CombatContext.Domain {
    internal sealed class ActiveDamagePacket {
        private readonly long packetId;
        private readonly Id<CharacterId> sourceCharacterId;
        private readonly Id<CharacterId> targetCharacterId;
        private readonly DamagePacketProcessingSettings settings;
        private readonly bool dispatchIncomingHitBeforeLayers;
        private ResolvedDamage currentDamage;
        private DamagePacketLayer nextLayer;
        private CombatTicks ticksUntilNextLayer;
        private bool completed;

        private ActiveDamagePacket(
            long packetId,
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            ResolvedDamage currentDamage,
            DamagePacketLayer firstLayer,
            bool dispatchIncomingHitBeforeLayers,
            DamagePacketProcessingSettings settings) {
            this.packetId = packetId;
            this.sourceCharacterId = NullGuard.ValidIdOrThrow(sourceCharacterId);
            this.targetCharacterId = NullGuard.ValidIdOrThrow(targetCharacterId);
            this.currentDamage = NullGuard.NotNullOrThrow(currentDamage);
            nextLayer = firstLayer;
            this.dispatchIncomingHitBeforeLayers = dispatchIncomingHitBeforeLayers;
            this.settings = NullGuard.NotNullOrThrow(settings);
            ticksUntilNextLayer = settings.getTicksPerStage();
        }

        internal static ActiveDamagePacket incomingAttack(
            long packetId,
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            DamageToDeal damageToDeal,
            DamagePacketProcessingSettings settings) {
            DamageToReceive damageToReceive = DamageToReceive.fromDamageToDeal(
                NullGuard.NotNullOrThrow(damageToDeal));
            return new ActiveDamagePacket(
                packetId,
                sourceCharacterId,
                targetCharacterId,
                ResolvedDamage.fromDamageToReceive(damageToReceive),
                DamagePacketLayer.Travel,
                true,
                settings);
        }

        internal static ActiveDamagePacket resolvedDamage(
            long packetId,
            Id<CharacterId> sourceCharacterId,
            Id<CharacterId> targetCharacterId,
            ResolvedDamage resolvedDamage,
            DamagePacketProcessingSettings settings) {
            return new ActiveDamagePacket(
                packetId,
                sourceCharacterId,
                targetCharacterId,
                resolvedDamage,
                DamagePacketLayer.Stability,
                false,
                settings);
        }

        internal bool isCompleted() {
            return completed;
        }

        internal void tick(CombatTicks combatTicks, CombatContext combatContext) {
            if (completed) {
                return;
            }

            CombatTicks availableTicks = combatTicks;
            while (availableTicks.isPositive() && !completed) {
                if (availableTicks < ticksUntilNextLayer) {
                    ticksUntilNextLayer -= availableTicks;
                    return;
                }

                availableTicks -= ticksUntilNextLayer;
                processNextLayer(combatContext);

                if (!completed) {
                    ticksUntilNextLayer = settings.getTicksPerStage();
                }
            }
        }

        private void processNextLayer(CombatContext combatContext) {
            switch (nextLayer) {
                case DamagePacketLayer.Travel:
                    processTravel(combatContext);
                    break;
                case DamagePacketLayer.Stability:
                    processStability(combatContext);
                    break;
                case DamagePacketLayer.Guard:
                    processGuard(combatContext);
                    break;
                case DamagePacketLayer.Hp:
                    processHp(combatContext);
                    break;
                default:
                    completed = true;
                    break;
            }
        }

        private void processTravel(CombatContext combatContext) {
            publishLayer(combatContext, DamagePacketLayer.Travel, currentDamage.getPower(), currentDamage.getPower());

            if (dispatchIncomingHitBeforeLayers) {
                publishLayer(
                    combatContext,
                    DamagePacketLayer.IncomingHit,
                    currentDamage.getPower(),
                    currentDamage.getPower(),
                    true);
                combatContext.dispatchCombatEvent(new IncomingAttackDamageCombatEvent(
                    targetCharacterId,
                    sourceCharacterId,
                    new DamageToDeal(currentDamage.getPower())));
                completed = true;
                return;
            }

            nextLayer = DamagePacketLayer.Stability;
        }

        private void processStability(CombatContext combatContext) {
            ICombatCharacterFacade target = combatContext.getCombatCharacterById(targetCharacterId);
            long incomingDamage = currentDamage.getPower();
            currentDamage = target.command().applyStabilityLayer(currentDamage);
            publishLayer(
                combatContext,
                DamagePacketLayer.Stability,
                incomingDamage,
                currentDamage.getPower(),
                currentDamage.getPower() <= 0);
            completeIfNoDamageOrContinue(DamagePacketLayer.Guard);
        }

        private void processGuard(CombatContext combatContext) {
            ICombatCharacterFacade target = combatContext.getCombatCharacterById(targetCharacterId);
            long incomingDamage = currentDamage.getPower();
            currentDamage = target.command().applyGuardLayer(currentDamage);
            publishLayer(
                combatContext,
                DamagePacketLayer.Guard,
                incomingDamage,
                currentDamage.getPower(),
                currentDamage.getPower() <= 0);
            completeIfNoDamageOrContinue(DamagePacketLayer.Hp);
        }

        private void processHp(CombatContext combatContext) {
            ICombatCharacterFacade target = combatContext.getCombatCharacterById(targetCharacterId);
            long incomingDamage = currentDamage.getPower();
            target.command().applyHpLayer(currentDamage);
            publishLayer(combatContext, DamagePacketLayer.Hp, incomingDamage, 0, true);
            completed = true;
        }

        private void completeIfNoDamageOrContinue(DamagePacketLayer nextLayerToProcess) {
            if (currentDamage.getPower() <= 0) {
                completed = true;
                return;
            }

            nextLayer = nextLayerToProcess;
        }

        private void publishLayer(
            CombatContext combatContext,
            DamagePacketLayer layer,
            long incomingDamage,
            long outgoingDamage,
            bool completesPacket = false) {
            combatContext.publishDamagePacketLayerProcessed(new DamagePacketLayerProcessedDtoEvent(
                packetId,
                sourceCharacterId,
                targetCharacterId,
                layer,
                incomingDamage,
                outgoingDamage,
                completesPacket ? 0 : settings.getTicksPerStage().getValue(),
                completesPacket));
        }
    }
}