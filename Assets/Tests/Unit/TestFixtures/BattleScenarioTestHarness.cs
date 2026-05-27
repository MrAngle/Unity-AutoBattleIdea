using System;
using System.Collections.Generic;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.Flow.Configuration;
using MageFactory.InjectConfiguration;
using MageFactory.Shared.Model;
using Zenject;

namespace MageFactory.Tests.Unit.TestFixtures {
    internal sealed class BattleScenarioTestHarness {
        private readonly DiContainer container;

        private readonly int enormousHp = 1_000_000;

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

        public BattleScenarioTestHarness withFlowSettings(int maxStepsPerSlice) {
            container.Rebind<FlowProcessorSettings>()
                .FromInstance(new FlowProcessorSettings(maxStepsPerSlice))
                .AsSingle();
            return this;
        }

        public ICombatContext createContext(params CreateCombatCharacterCommand[] commands) {
            var factory = container.Resolve<ICombatContextFactory>();
            return factory.create(commands);
        }

        public ICombatContext create1V1WithEnormousHp(
            IReadOnlyList<EquipItemCommand> attackerItems,
            IReadOnlyList<EquipItemCommand> defenderItems = null,
            string attackerName = "Attacker",
            string defenderName = "Defender") {
            defenderItems ??= Array.Empty<EquipItemCommand>();

            return createContext(
                new CreateCombatCharacterCommand(attackerName, enormousHp, Team.TeamA, attackerItems),
                new CreateCombatCharacterCommand(defenderName, enormousHp, Team.TeamB, defenderItems)
            );
        }
    }
}