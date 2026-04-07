using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Configuration;
using MageFactory.Flow.Domain;
using MageFactory.InjectConfiguration;
using MageFactory.Shared.Model;
using Zenject;

namespace MageFactory.Tests.Unit.TestFixtures {
    internal sealed class BattleScenarioTestHarness {
        private readonly DiContainer container;

        private const int EnormousHp = 1_000_000;

        private BattleScenarioTestHarness(DiContainer container) {
            this.container = container;
        }

        public static BattleScenarioTestHarness create() {
            var container = new DiContainer();

            MageFactoryDomainInstaller.Install(container);

            container.DeclareSignal<ItemRemovedDtoEvent>();
            container.DeclareSignal<ItemPowerChangedDtoEvent>();

            var signalBus = container.Resolve<SignalBus>();
            signalBus.Subscribe<ItemPowerChangedDtoEvent>(_ => { });
            signalBus.Subscribe<ItemRemovedDtoEvent>(_ => { });

            return new BattleScenarioTestHarness(container);
        }

        public BattleScenarioTestHarness withInstantActionExecutorInstance() {
            container.Rebind<IActionExecutor>().FromInstance(new InstantActionExecutor()).AsSingle();
            return this;
        }

        public BattleScenarioTestHarness withActionExecutorInstance(IActionExecutor executor) {
            container.Rebind<IActionExecutor>().FromInstance(executor).AsSingle();
            return this;
        }

        public BattleScenarioTestHarness withFlowSettings(int maxStepsPerSlice) {
            container.Rebind<FlowProcessorSettings>()
                .FromInstance(new FlowProcessorSettings(maxStepsPerSlice))
                .AsSingle();
            return this;
        }

        public BattleScenarioTestHarness withStepScheduler(IFlowStepScheduler scheduler) {
            container.Rebind<IFlowStepScheduler>()
                .FromInstance(scheduler)
                .AsSingle();
            return this;
        }

        public ICombatContext createContext(params CreateCombatCharacterCommand[] commands) {
            var factory = container.Resolve<ICombatContextFactory>();
            return factory.create(commands);
        }

        // NEW: prosty scenariusz 1v1, HP nie ma znaczenia w testach (ogromne)
        public ICombatContext create1V1WithEnormousHp(
            IReadOnlyList<EquipItemCommand> attackerItems,
            IReadOnlyList<EquipItemCommand> defenderItems = null,
            string attackerName = "Attacker",
            string defenderName = "Defender") {
            defenderItems ??= Array.Empty<EquipItemCommand>();

            return createContext(
                new CreateCombatCharacterCommand(attackerName, EnormousHp, Team.TeamA, attackerItems),
                new CreateCombatCharacterCommand(defenderName, EnormousHp, Team.TeamB, defenderItems)
            );
        }
    }

    public sealed class InstantActionExecutor : IActionExecutor {
        public Task executeAsync(ExecuteActionCommand actionCommand) {
            var effects = actionCommand.itemActionDescription
                .getEffectsDescriptor()
                .getEffects();

            for (var i = 0; i < effects.Count; i++) {
                effects[i].apply(actionCommand.actionCapabilities);
            }

            return Task.CompletedTask;
        }
    }
}