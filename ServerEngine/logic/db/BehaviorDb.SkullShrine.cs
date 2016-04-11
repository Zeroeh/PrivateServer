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
        static _ SkullShrine = Behav()
            .Init(0x0d56, Behaves("Skull Shrine",
                    NullBehavior.Instance,
                    Cooldown.Instance(750, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 9, 1)
                    ),
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 6, 0, 2,
                            Tuple.Create(0.05, (ILoot)new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.06, (ILoot)new TierLoot(10, ItemType.Armor)),

                            Tuple.Create(0.09, (ILoot)new TierLoot(9, ItemType.Weapon)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.09, (ILoot)new TierLoot(4, ItemType.Ring))
                        )),
                        Tuple.Create(100, new LootDef(1, 3, 1, 3,
                            Tuple.Create(0.05, (ILoot)new StatPotionsLoot(1, 2))
                        ))
                    )
                ))
				.Init(0x2237, Behaves("Skull Shrine Super",
					NullBehavior.Instance,
					Cooldown.Instance(750, PredictiveMultiAttack.Instance(40, 5 * (float)Math.PI / 180, 14, 1)
					),
					loot: new LootBehavior(LootDef.Empty,
						Tuple.Create(100, new LootDef(0, 6, 0, 2,
							Tuple.Create(0.01, (ILoot)new ItemLoot("Orb of Conflict")),

							Tuple.Create(0.01, (ILoot)new TierLoot(11, ItemType.Weapon)),
							Tuple.Create(0.01, (ILoot)new TierLoot(11, ItemType.Armor)),
							Tuple.Create(0.03, (ILoot)new TierLoot(5, ItemType.Ring))
						)),
						Tuple.Create(100, new LootDef(1, 3, 1, 3,
							Tuple.Create(0.05, (ILoot)new StatPotionsLoot(1, 2))
						))
					)
				))
            .Init(0x0d57, Behaves("Red Flaming Skull",
                    IfNot.Instance(
                        Chasing.Instance(2, 20, 3, 0x0d56),
                        SimpleWandering.Instance(2, .5f)
                    ),
                    Cooldown.Instance(750, PredictiveMultiAttack.Instance(15, 5 * (float)Math.PI / 180, 2, 1))
                ))
            .Init(0x0d58, Behaves("Blue Flaming Skull",
                    IfNot.Instance(
                        Circling.Instance(15, 20, 20, 0x0d56),
                        SimpleWandering.Instance(20)
                    ),
                    Cooldown.Instance(750, PredictiveMultiAttack.Instance(15, 5 * (float)Math.PI / 180, 2, 1))
                ));
    }
}
