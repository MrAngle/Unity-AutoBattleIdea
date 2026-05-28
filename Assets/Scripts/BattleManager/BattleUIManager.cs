using System;
using System.Collections;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Item.Catalog.Bases;
using MageFactory.Shared.Model;
using MageFactory.Shared.Utility;
using MageFactory.UI.Context.Combat;
using UnityEngine;
using Zenject;

namespace MageFactory.BattleManager {
    public class BattleUIManager : MonoBehaviour {
        private static readonly GridDimensions DefaultInventoryGridDimensions = new GridDimensions(17, 8);

        private BattleRuntime battleRuntime; // move to BattleManager
        private Coroutine battleLoop;

        private ICombatContextFactory combatContextFactory;
        private CombatContextPresentationFactory combatContextPresentationFactory;
        private BattleSessionSettings battleSessionSettings;
        private ICombatContext combatContext;

        private BattleSession battleSession;

        private void Start() {
            createSlots(new CreateCombatCharacterCommand[] {
                new("Warrior", 120, Team.TeamA, DefaultInventoryGridDimensions, new[] {
                    new EquipItemCommand(
                        new EntryPointGem(), new Vector2Int(0, 0))
                }),
                new("Mage", 1220, Team.TeamB, DefaultInventoryGridDimensions, Array.Empty<EquipItemCommand>()),
                new("Archer", 1300, Team.TeamB, DefaultInventoryGridDimensions, Array.Empty<EquipItemCommand>())
            });

            startBattleLoop();
        }

        [Inject]
        public void construct(BattleRuntime injectBattleRuntime,
                              ICombatContextFactory injectCombatContextFactory,
                              CombatContextPresentationFactory injectCombatContextPresentationFactory,
                              BattleSessionSettings injectBattleSessionSettings) {
            battleRuntime = NullGuard.NotNullOrThrow(injectBattleRuntime);
            combatContextFactory = NullGuard.NotNullOrThrow(injectCombatContextFactory);
            combatContextPresentationFactory = NullGuard.NotNullOrThrow(injectCombatContextPresentationFactory);
            battleSessionSettings = NullGuard.NotNullOrThrow(injectBattleSessionSettings);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            combatContext = combatContextFactory.create(charactersToCreate);
            battleSession = new BattleSession(battleRuntime, combatContext, battleSessionSettings);

            combatContextPresentationFactory.createCombatContextPresentation(combatContext);
        }

        private void startBattleLoop() {
            battleLoop = StartCoroutine(executeLoop());
        }

        private IEnumerator executeLoop() {
            WaitForSeconds wait = new WaitForSeconds(battleSession.getSettings().getRealSecondsPerCombatTick());

            while (true) {
                battleSession.tickOnce();
                yield return wait;
            }
        }
    }
}