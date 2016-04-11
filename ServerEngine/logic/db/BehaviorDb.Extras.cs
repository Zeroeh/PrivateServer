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
        static _ Extras = Behav()
            .Init(0x1729, Behaves("Armor Guard",
                new RunBehaviors(
                    Chasing.Instance(.5f, 6, 0, null), SimpleWandering.Instance(4)),
                    Cooldown.Instance(1000, SimpleAttack.Instance(3))
					))
			.Init(0x1741, Behaves("Assassin of Oryx",
                    IfNot.Instance(
                    Chasing.Instance(7.5f, 6, 0, null), SimpleWandering.Instance(4)),
                    Cooldown.Instance(200, SimpleAttack.Instance(10)),
                    loot: new LootBehavior(
                        new LootDef(1, 2, 1, 8,
                            Tuple.Create(0.9, (ILoot)new TierLoot(1, ItemType.Weapon))
                        )
                    )
                    ));
    }
}