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
        
        private FlowCompletionDispatcher(FlowModel flowModel)
        {
            _flowModel = flowModel;
        }

        private void Handle() {
            // for now
            var teamA = CharacterRegistry.Instance.GetTeamA();
            var teamB = CharacterRegistry.Instance.GetTeamB();

            // wybierz losowego atakującego z drużyny A
            var attacker = CharacterRegistry.Instance.GetTeamA()[Random.Range(0, teamA.Count)];
            var target = CharacterRegistry.Instance.GetTeamB()[Random.Range(0, teamB.Count)];

            var dmg = _flowModel.FlowPayload.Power;
            target.TakeDamage(dmg);

            Debug.Log($"{attacker.Name} zadał {dmg} obrażeń {target.Name}");
        }
        

    }
}