using System;
using System.Collections.Generic;
using MageFactory.Flow.Contract;
using MageFactory.FlowRouting;
using MageFactory.Shared.Contract;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;

namespace MageFactory.Flow.Domain {
    internal class FlowContext {
        private readonly FlowKind flowKind;
        private readonly IFlowItem startEntryPoint;
        private readonly IFlowConsumer flowConsumer;
        private readonly IFlowOwner flowOwner;
        private readonly IFlowRouter router;

        private readonly Dictionary<DamageRole, FlowEffectBucket> flowEffectBucketsByDamageRole = new();

        private int stepIndex;

        internal FlowContext(FlowKind flowKind,
                             IFlowItem startEntryPoint,
                             IFlowConsumer flowConsumer,
                             IFlowOwner flowOwner,
                             IFlowRouter router) {
            this.flowKind = NullGuard.enumDefinedOrThrow(flowKind);
            this.startEntryPoint = NullGuard.NotNullOrThrow(startEntryPoint);
            this.flowConsumer = NullGuard.NotNullOrThrow(flowConsumer);
            this.flowOwner = NullGuard.NotNullOrThrow(flowOwner);
            this.router = NullGuard.NotNullOrThrow(router);

            NullGuard.NotNullCheckOrThrow(this.startEntryPoint, this.flowConsumer, this.flowOwner,
                this.router, flowEffectBucketsByDamageRole);
        }

        internal IFlowConsumer getFlowConsumer() {
            return flowConsumer;
        }

        internal IFlowOwner getFlowOwner() {
            return flowOwner;
        }

        internal IFlowRouter getFlowRouter() {
            return router;
        }

        internal FlowKind getFlowKind() {
            return flowKind;
        }

        internal PowerAmount getAttackPower() {
            if (flowEffectBucketsByDamageRole.TryGetValue(DamageRole.ATTACK, out FlowEffectBucket existingBucket)) {
                return existingBucket.getPower(); // maybe it shoudl be like tryGetPower? Keep it simple for now
            }

            return PowerAmount.noPower();
        }

        internal void changeDamagePower(DamageRole damageRole, PowerAmount powerDelta) {
            if (powerDelta == null) {
                throw new ArgumentNullException(nameof(powerDelta));
            }

            if (powerDelta.getPower() == 0) {
                return;
            }

            FlowEffectBucket bucket = getOrCreateBucketOrNull(damageRole);
            if (bucket == null) {
                return;
            }

            bucket.changePower(powerDelta);

            if (bucket.hasNoEffect()) {
                flowEffectBucketsByDamageRole.Remove(damageRole);
            }
        }

        private FlowEffectBucket getOrCreateBucketOrNull(DamageRole damageRole) {
            if (flowEffectBucketsByDamageRole.TryGetValue(damageRole, out FlowEffectBucket existingBucket)) {
                return existingBucket;
            }

            FlowEffectBucket newBucket = new FlowEffectBucket(damageRole, PowerAmount.noPower());
            flowEffectBucketsByDamageRole.Add(damageRole, newBucket);
            return newBucket;
        }
    }
}