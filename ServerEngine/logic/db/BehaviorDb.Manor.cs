using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.logic.attack;
using ServerEngine.logic.movement;
using ServerEngine.logic.loot;
using ServerEngine.logic.taunt;
using ServerEngine.logic.cond;

namespace ServerEngine.logic
{
    partial class BehaviorDb
    {
        static _ Manor = Behav()
            .Init(0x1720, Behaves("Lord Ruthven",
                    new RunBehaviors(
                        SimpleWandering.Instance(2, 2),
                        Timed.Instance(4500, Cooldown.Instance(350, RingAttack.Instance(20, 0, 5, projectileIndex: 1))),
                        Cooldown.Instance(1100, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 4, 0, projectileIndex: 0))
                    ),

                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 0, 2,
                            Tuple.Create(0.1, (ILoot)new ItemLoot("Wine Cellar Incantation")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Tome of Purification")),
                            Tuple.Create(0.5, (ILoot)new ItemLoot("Holy Water")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("St. Abraham's Wand")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Chasuble of Holy Light")),
                            Tuple.Create(0.12, (ILoot)new ItemLoot("Ring of Divine Faith")),
                            Tuple.Create(1.0, (ILoot)new StatPotionLoot(StatPotion.Atk))
                    )))

            ));
    }
}
