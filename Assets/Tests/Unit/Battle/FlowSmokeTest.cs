using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.ActionExecutor.Api;
using MageFactory.ActionExecutor.Api.Dto;
using MageFactory.BattleManager;
using MageFactory.Character.Contract.Event;
using MageFactory.CombatContext.Api;
using MageFactory.CombatContext.Contract.Command;
using MageFactory.InjectConfiguration;
using MageFactory.Shared.Model;
using MageFactory.Shared.Model.Shape;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace MageFactory.Tests.Unit.Battle {
    public sealed class DeterministicFlowScenarioTest {
        [Test]
        public void should_run_deterministic_linear_flow_and_damage_enemy_in_edit_mode_without_time() {
            // arrange: container (bez sceny)
            var container = new DiContainer();

            MageFactoryDomainInstaller.Install(container);

            // SignalBus.Fire też wymaga DeclareSignal<T>(), nawet jeśli nikt nie subskrybuje.
            // W domenie ActionCommandBus.Fire(...) odpala sygnały itemowe.
            container.DeclareSignal<ItemRemovedDtoEvent>();
            container.DeclareSignal<ItemPowerChangedDtoEvent>();

            // deterministycznie: ignorujemy cast time
            container.Rebind<IActionExecutor>().To<InstantActionExecutor>().AsSingle();

            var ctxFactory = container.Resolve<ICombatContextFactory>();

            // deterministyczny łańcuch 1x1: (0,0)->(1,0)->(2,0)->(3,0)
            // Router w każdym kroku ma dokładnie 1 kandydata, więc brak losowości.
            var commands = new List<CreateCombatCharacterCommand> {
                new CreateCombatCharacterCommand(
                    name: "Attacker",
                    maxHp: 100,
                    team: Team.TeamA,
                    itemsToEquip: new List<EquipItemCommand> {
                        new EquipItemCommand(new TestEntryPoint(power: 2), new Vector2Int(0, 0)),
                        new EquipItemCommand(new TestDamageItem(power: 3), new Vector2Int(1, 0)),
                        new EquipItemCommand(new TestDamageItem(power: 4), new Vector2Int(2, 0)),
                        new EquipItemCommand(new TestDamageItem(power: 5), new Vector2Int(3, 0))
                    }
                ),
                new CreateCombatCharacterCommand(
                    name: "Defender",
                    maxHp: 50,
                    team: Team.TeamB,
                    itemsToEquip: Array.Empty<EquipItemCommand>()
                )
            };

            var context = ctxFactory.create(commands);

            var runtime = new BattleRuntime();
            var session = new BattleSession(runtime, context);

            var defenderHpBefore = getTeamHp(context, Team.TeamB);

            // act
            session.tickOnce();
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(5)) {
                var hp = getTeamHp(context, Team.TeamB);
                if (hp < defenderHpBefore) break;

                Thread.Sleep(10);
            }

            var defenderHpAfter = getTeamHp(context, Team.TeamB);

            // assert
            Assert.That(defenderHpAfter, Is.LessThan(defenderHpBefore),
                "Flow nie zadał obrażeń (albo nie został skonsumowany), mimo deterministycznego łańcucha.");
        }

        private static long getTeamHp(ICombatContext ctx, Team team) {
            foreach (var ch in ctx.getAllCharacters()) {
                if (ch.getTeam() == team)
                    return ch.getCurrentHp();
            }

            throw new InvalidOperationException($"No character for team {team}.");
        }

        /// <summary>
        /// Executor testowy: wykonuje efekty natychmiast (bez ContinueIn/czasu).
        /// Dzięki temu test jest deterministyczny w EditMode.
        /// </summary>
        private sealed class InstantActionExecutor : IActionExecutor {
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

        private sealed class TestEntryPoint : IEntryPointDefinition {
            private readonly int power;

            public TestEntryPoint(int power) {
                this.power = power;
            }

            public ShapeArchetype getShape() => ShapeCatalog.Square1x1;

            public IActionDescription getActionDescription() => new ActionDesc(power);

            public FlowKind getFlowKind() => FlowKind.Damage;

            private sealed class ActionDesc : IActionDescription {
                private readonly int power;

                public ActionDesc(int power) {
                    this.power = power;
                }

                public Duration getCastTime() => new Duration(0f);

                public IOperations getEffectsDescriptor() => new Ops(
                    new AddPower(new DamageToDeal(power))
                );
            }
        }

        private sealed class TestDamageItem : IItemDefinition {
            private readonly int power;

            public TestDamageItem(int power) {
                this.power = power;
            }

            public ShapeArchetype getShape() => ShapeCatalog.Square1x1;

            public IActionDescription getActionDescription() => new ActionDesc(power);

            private sealed class ActionDesc : IActionDescription {
                private readonly int power;

                public ActionDesc(int power) {
                    this.power = power;
                }

                public Duration getCastTime() => new Duration(0f);

                public IOperations getEffectsDescriptor() => new Ops(
                    new AddPower(new DamageToDeal(power))
                );
            }
        }

        private sealed class Ops : IOperations {
            private readonly IReadOnlyList<IOperation> effects;

            public Ops(params IOperation[] effects) {
                this.effects = effects ?? Array.Empty<IOperation>();
            }

            public IReadOnlyList<IOperation> getEffects() => effects;
        }
    }
}