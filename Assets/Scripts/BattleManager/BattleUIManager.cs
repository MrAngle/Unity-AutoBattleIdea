using System;
using System.Collections;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Item.Catalog;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat;
using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleUIManager : MonoBehaviour {
        private BattleRuntime battleRuntime; // move to BattleManager
        private Coroutine battleLoop;

        private ICombatContextFactory combatContextFactory;
        private CombatContextPresentationFactory combatContextPresentationFactory;
        private ICombatContext combatContext;

        private float turnInterval = 2f;

        private void Start() {
            createSlots(new CreateCombatCharacterCommand[] {
                new("Warrior", 120, Team.TeamA, new[] {
                    new EquipItemCommand(
                        EntryPointDefinition.Standard, new Vector2Int(0, 0))
                }),
                new("Mage", 1220, Team.TeamB, Array.Empty<EquipItemCommand>()),
                new("Archer", 1300, Team.TeamB, Array.Empty<EquipItemCommand>())
            });

            startBattleLoop();
        }

        [Inject]
        public void construct(BattleRuntime injectBattleRuntime,
                              ICombatContextFactory injectCombatContextFactory,
                              CombatContextPresentationFactory injectCombatContextPresentationFactory) {
            battleRuntime = NullGuard.NotNullOrThrow(injectBattleRuntime);
            combatContextFactory = NullGuard.NotNullOrThrow(injectCombatContextFactory);
            combatContextPresentationFactory = NullGuard.NotNullOrThrow(injectCombatContextPresentationFactory);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            combatContext = combatContextFactory.create(charactersToCreate);

            combatContextPresentationFactory.createCombatContextPresentation(combatContext);
        }

        private void startBattleLoop() {
            battleLoop = StartCoroutine(executeLoop());
        }

        private IEnumerator executeLoop() {
            var wait = new WaitForSeconds(turnInterval);

            while (true) {
                battleRuntime.tick(combatContext);
                yield return wait;
            }
        }
    }
}