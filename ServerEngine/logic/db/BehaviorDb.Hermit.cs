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
        static _ Hermit = Behav()
.Init(0x0d61, Behaves("Hermit God",
                    new RunBehaviors(
                        IfExist.Instance(-1, NullBehavior.Instance,
                            new RunBehaviors(
                                new QueuedBehavior(
                                    CooldownExact.Instance(400)
                                ),
                                SimpleWandering.Instance(2),
                                new QueuedBehavior(new SetKey(-1, 1))

                            )
                        ),
                        IfEqual.Instance(-1, 1,
                            new RunBehaviors(
                            SimpleWandering.Instance(2),
                            Cooldown.Instance(200, MultiAttack.Instance(15, 10 * (float)Math.PI / 180, 4, 0, projectileIndex: 0)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x0d62, 6, 15, 15))
                            )
                        )),

                    loot: new LootBehavior(LootDef.Empty,
                            Tuple.Create(100, new LootDef(0, 2, 0, 2,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Helm of the Juggernaut")),
                            Tuple.Create(0.8, (ILoot)new StatPotionLoot(StatPotion.Dex)),
                            Tuple.Create(0.8, (ILoot)new StatPotionLoot(StatPotion.Vit))
                    )))

            ))
            .Init(0x0d64, Behaves("Hermit God Tentacle",
            new RunBehaviors(
                        Cooldown.Instance(700, RingAttack.Instance(8, 100, 0, projectileIndex: 0)),
                        Circling.Instance(5, 7, 4, 0x0d61),
                        Chasing.Instance(4, 3, 3, null)
                        )


        ))
        .Init(0x0d63, Behaves("Whirlpool",
            new RunBehaviors(
                        Cooldown.Instance(700, RingAttack.Instance(8, 100, 0, projectileIndex: 0)),
                        Circling.Instance(5, 7, 3, 0x0d61)
                        )


        ))
        .Init(0x0d62, Behaves("Hermit Minion",
            new RunBehaviors(
                        MaintainDist.Instance(4, 7, 7, 0x0d61),
                        Cooldown.Instance(700, SimpleAttack.Instance(8, projectileIndex: 0)),
                        Cooldown.Instance(1000, SimpleAttack.Instance(8, projectileIndex: 1)),
                        SimpleWandering.Instance(3)
                        )


        ));
    }
}
