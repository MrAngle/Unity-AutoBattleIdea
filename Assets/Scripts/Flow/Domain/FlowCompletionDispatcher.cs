using MageFactory.Registry;
using UnityEngine;

namespace MageFactory.Flow.Domain {
    internal class FlowCompletionDispatcher {
        private readonly FlowModel flowModel;

        private FlowCompletionDispatcher(FlowModel flowModel) {
            this.flowModel = flowModel;
        }

        internal static void finishFlow(FlowModel flowModel) {
            FlowCompletionDispatcher flowCompletionDispatcher = new FlowCompletionDispatcher(flowModel);

            // TODO: change it
            flowCompletionDispatcher.handleFlow();
        }

        // todo: refactor, shouldnt base on registry
        private void handleFlow() {
            // for now
            var teamA = CharacterRegistry.Instance.getTeamA();
            var teamB = CharacterRegistry.Instance.getTeamB();

            // wybierz losowego atakującego z drużyny A
            var attacker = CharacterRegistry.Instance.getTeamA()[Random.Range(0, teamA.Count)];
            var target = CharacterRegistry.Instance.getTeamB()[Random.Range(0, teamB.Count)];

            var damageToDeal = flowModel.getFlowPayload().getDamageToDeal();
            var damageToReceive = flowModel.getFlowPayload().getDamageToReceive();
            target.apply(damageToDeal);
            // attacker.Apply(damageToReceive); // for now

            Debug.Log($"{attacker.getName()} zadał {damageToDeal.getPower()} obrażeń {target.getName()}");
        }
    }
}