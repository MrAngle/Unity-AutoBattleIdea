using System;
using System.Collections.Generic;
using MageFactory.ActionEffect;
using MageFactory.ActionEffect.PredefinedOperations;
using MageFactory.CombatContext.Api;
using MageFactory.Shared.Model;

namespace MageFactory.Tests.Unit.TestFixtures {
    public static class TestHelpers {
        public static long getDamage(IEnumerable<IItemDefinition> itemDefinitions) {
            long totalDamage = 0;

            foreach (var itemDef in itemDefinitions) {
                if (itemDef == null) continue;

                IActionDescription actionDesc = itemDef.getActionDescription();
                if (actionDesc == null) continue;

                var effects = actionDesc.getEffectsDescriptor()?.getEffects();
                if (effects == null) continue;


                foreach (var effect in effects) {
                    if (effect is not AddPower addPower) {
                        continue;
                    }

                    PowerAmount powerAmount = addPower.getDamageAmount();
                    totalDamage += powerAmount.getPower();

                    if (totalDamage < 0) {
                        totalDamage = 0;
                    }
                }
            }

            return totalDamage;
        }

        public static long getTeamHp(ICombatContext ctx, Team team) {
            foreach (var ch in ctx.getAllCharacters()) {
                if (ch.query().getCharacterInfo().getTeam() == team)
                    return ch.query().getCharacterInfo().getCurrentHp();
            }

            throw new InvalidOperationException($"No character for team {team}.");
        }
    }
}