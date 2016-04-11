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
        static _ RedDemon = Behav()
            .Init(0x668, Behaves("Red Demon",
                    new QueuedBehavior(
                        Timed.Instance(5000, False.Instance(SmoothWandering.Instance(1f, 3f))),
                        Timed.Instance(5000, Not.Instance(Chasing.Instance(3f, 10, 2, null))),
                        Not.Instance(ReturnSpawn.Instance(3f))
                    ),
                    /*new RunBehaviors(
                        Once.Instance(
                            new RunBehaviors(
                                SpawnMinionImmediate.Instance(0x669, 1, 1, 3),
                                SpawnMinionImmediate.Instance(0x66a, 1, 1, 3),
                                SpawnMinionImmediate.Instance(0x66b, 1, 1, 3)
                            )
                        ),
                        new QueuedBehavior(
                            Rand.Instance(
                                SpawnMinionImmediate.Instance(0x669, 1, 1, 3),
                                SpawnMinionImmediate.Instance(0x66a, 1, 1, 3),
                                SpawnMinionImmediate.Instance(0x66b, 1, 1, 3),
                                NullBehavior.Instance
                            ),
                            True.Instance(Rand.Instance(
                                new RandomTaunt(0.1, "I will deliver your soul to Oryx, {PLAYER}!"),
                                new RandomTaunt(0.1, "Oryx will not end our pain. We can only share it... with you!"),
                                new RandomTaunt(0.1, "Our anguish is endless, unlike your lives!"),
                                new RandomTaunt(0.1, "There can be no forgiveness!"),
                                new RandomTaunt(0.1, "What do you know of suffering? I can teach you much, {PLAYER}"),
                                new RandomTaunt(0.1, "Would you attempt to destroy us? I know your name, {PLAYER}!"),
                                new RandomTaunt(0.1, "You cannot hurt us. You cannot help us. You will feed us."),
                                new RandomTaunt(0.1, "Your life is an offering to Oryx. You will die.")
                            )),
                            Cooldown.Instance(2000)
                        ),*/
                    new QueuedBehavior(
                        Cooldown.Instance(500, SimpleAttack.Instance(10, projectileIndex: 1)),
                        Cooldown.Instance(1000, PredictiveMultiAttack.Instance(10, 5 * (float)Math.PI / 180, 5, 1, projectileIndex: 0))
                    ),
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 0, 8,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Sword of Illumination")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Bow of the Morning Star")),
                            Tuple.Create(0.5, (ILoot)HpPotionLoot.Instance),
                            Tuple.Create(0.5, (ILoot)MpPotionLoot.Instance),
                            Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Spd)),
                            Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Atk))
                        )))
                ));
            /*.Init(0x669, Behaves("Imp",
                    SimpleWandering.Instance(7f),
                    Cooldown.Instance(250, SimpleAttack.Instance(10)),
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 0, 8,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Fire Bow")),
                            Tuple.Create(0.5, (ILoot)HpPotionLoot.Instance)
                        ))
                    )
                ))
            .Init(0x66a, Behaves("Demon",
                    SimpleWandering.Instance(7f),
                    Cooldown.Instance(1000, PredictiveMultiAttack.Instance(10, 5 * (float)Math.PI / 180, 3, 0.5f)),
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 0, 8,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Fire Bow")),
                            Tuple.Create(0.5, (ILoot)HpPotionLoot.Instance)

                        ))
                    )
                ))
            .Init(0x66b, Behaves("Demon Warrior",
                    SimpleWandering.Instance(7f),
                    Cooldown.Instance(1000, PredictiveMultiAttack.Instance(10, 5 * (float)Math.PI / 180, 3, 0.5f)),
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 0, 8,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Obsidian Dagger")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Steel Shield")),
                            Tuple.Create(0.5, (ILoot)HpPotionLoot.Instance)
                            
                        ))
                    )
                ));*/
    }
}
