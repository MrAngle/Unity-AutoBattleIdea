using Registry;
using UnityEngine;

namespace Combat.Flow.Domain.Aggregate {
    public class FlowCompletionDispatcher {
        private readonly FlowModel _flowModel;

        public static void Finish(FlowModel flowModel) {
            FlowCompletionDispatcher flowCompletionDispatcher = new FlowCompletionDispatcher(flowModel);

            // TODO: change it
            flowCompletionDispatcher.Handle();
        }

        private FlowCompletionDispatcher(FlowModel flowModel) {
            _flowModel = flowModel;
        }

        private void Handle() {
            // for now
            var teamA = CharacterRegistry.Instance.GetTeamA();
            var teamB = CharacterRegistry.Instance.GetTeamB();

            // wybierz losowego atakującego z drużyny A
            var attacker = CharacterRegistry.Instance.GetTeamA()[Random.Range(0, teamA.Count)];
            var target = CharacterRegistry.Instance.GetTeamB()[Random.Range(0, teamB.Count)];

            var damageToDeal = _flowModel.FlowPayload.GetDamageToDeal();
            var damageToReceive = _flowModel.FlowPayload.GetDamageToReceive();
            target.Apply(damageToDeal);
            // attacker.Apply(damageToReceive); // for now

            Debug.Log($"{attacker.GetName()} zadał {damageToDeal.GetPower()} obrażeń {target.GetName()}");
        }
    }
}