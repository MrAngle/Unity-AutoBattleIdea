using System;
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

        private ICombatContextFactory combatContextFactory;
        private CombatContextPresentationFactory combatContextPresentationFactory;
        private BattleSessionSettings battleSessionSettings;
        private BattleTimeController battleTimeController;
        private Transform battleSlotParent;
        private ICombatContext combatContext;

        private BattleSession battleSession;

        private void Start() {
            createSlots(new CreateCombatCharacterCommand[] {
                new("Warrior", 120, Team.TeamA, DefaultInventoryGridDimensions, new[] {
                    new EquipItemCommand(
                        new EntryPointGem(), new Vector2Int(0, 0)),
                    new EquipItemCommand(
                        new BasicOutputPort(), new Vector2Int(1, 0)),
                    new EquipItemCommand(
                        new PulseInputPort(), new Vector2Int(0, 1)),
                    new EquipItemCommand(
                        new BasicOutputPort(), new Vector2Int(1, 1))
                }),
                new("Mage", 1220, Team.TeamB, DefaultInventoryGridDimensions, Array.Empty<EquipItemCommand>()),
                new("Archer", 1300, Team.TeamB, DefaultInventoryGridDimensions, Array.Empty<EquipItemCommand>())
            });

            createBattleTimeControls();
        }

        private void Update() {
            if (battleSession == null || battleTimeController == null) {
                return;
            }

            int ticksToRun = battleTimeController.consumeReadyCombatTicks(Time.unscaledDeltaTime);

            if (ticksToRun <= 0) {
                return;
            }

            for (int i = 0; i < ticksToRun; i++) {
                battleSession.tickOnce();
            }

            combatContextPresentationFactory.refreshCastProgress();
        }

        [Inject]
        public void construct(BattleRuntime injectBattleRuntime,
                              ICombatContextFactory injectCombatContextFactory,
                              CombatContextPresentationFactory injectCombatContextPresentationFactory,
                              BattleSessionSettings injectBattleSessionSettings,
                              [Inject(Id = "BattleSlotParent")] Transform injectBattleSlotParent) {
            battleRuntime = NullGuard.NotNullOrThrow(injectBattleRuntime);
            combatContextFactory = NullGuard.NotNullOrThrow(injectCombatContextFactory);
            combatContextPresentationFactory = NullGuard.NotNullOrThrow(injectCombatContextPresentationFactory);
            battleSessionSettings = NullGuard.NotNullOrThrow(injectBattleSessionSettings);
            battleSlotParent = NullGuard.NotNullOrThrow(injectBattleSlotParent);
            battleTimeController = new BattleTimeController(battleSessionSettings);
        }

        private void createSlots(CreateCombatCharacterCommand[] charactersToCreate) {
            combatContext = combatContextFactory.create(charactersToCreate);
            battleSession = new BattleSession(battleRuntime, combatContext, battleSessionSettings);

            combatContextPresentationFactory.createCombatContextPresentation(combatContext);
        }

        private void createBattleTimeControls() {
            if (battleSlotParent.parent is RectTransform canvasRoot) {
                BattleTimeControlsView.create(canvasRoot, battleTimeController);
                return;
            }

            if (battleSlotParent is RectTransform battleSlotRectTransform) {
                BattleTimeControlsView.create(battleSlotRectTransform, battleTimeController);
            }
        }
    }
}