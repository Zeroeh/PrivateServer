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
        static _ CubeGod = Behav()
            .Init(0x0d59, Behaves("Cube God",
                    SimpleWandering.Instance(1, .5f),
                    new RunBehaviors(
                        Cooldown.Instance(750, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 9, 1, projectileIndex: 0)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 4, 1, projectileIndex: 1))
                        ),
                        loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 3, 0, 8,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Dirk of Cronus")),

                            Tuple.Create(0.02, (ILoot)new TierLoot(11, ItemType.Weapon)),
                            Tuple.Create(0.01, (ILoot)new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.01, (ILoot)new TierLoot(5, ItemType.Ring)),

                            Tuple.Create(0.03, (ILoot)new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.03, (ILoot)new TierLoot(10, ItemType.Armor)),

                            Tuple.Create(0.04, (ILoot)new TierLoot(9, ItemType.Weapon)),
                            Tuple.Create(0.02, (ILoot)new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.06, (ILoot)new TierLoot(9, ItemType.Armor)),

                            //Tuple.Create(0.05, (ILoot)new StatPotionsLoot(1, 2)), gives WAY too many potions
                            Tuple.Create(0.6, (ILoot)new TierLoot(4, ItemType.Ring)),

                            Tuple.Create(0.6, (ILoot)new TierLoot(4, ItemType.Ability))
                        ))
                    )
                ))
            .Init(0x0d5a, Behaves("Cube Overseer",
                    IfNot.Instance(
                        Circling.Instance(5, 25, 4, 0x0d59),
                        SimpleWandering.Instance(2)
                    ),
                    new RunBehaviors(
                        Cooldown.Instance(1000, PredictiveMultiAttack.Instance(10, 5 * (float)Math.PI / 180, 4, 1, projectileIndex: 0)),
                        Cooldown.Instance(2000, PredictiveAttack.Instance(10, 1, projectileIndex: 1))
                    )
                ))
            .Init(0x0d5b, Behaves("Cube Defender",
                    IfNot.Instance(
                        IfEqual.Instance(-1, 0,
                            Chasing.Instance(7, 20, 1, null),
                            IfNot.Instance(
                                Chasing.Instance(7, 25, 7.5f, 0x0d5a),
                                Circling.Instance(7.5f, 25, 7, 0x0d59)
                            )
                        ),
                        SimpleWandering.Instance(5)
                    ),
                    Cooldown.Instance(500, SimpleAttack.Instance(10))
                ))
            .Init(0x0d5c, Behaves("Cube Blaster",
                    IfNot.Instance(
                        IfEqual.Instance(-1, 0,
                            Chasing.Instance(7, 20, 1, null),
                            IfNot.Instance(
                                Chasing.Instance(7, 25, 7.5f, 0x0d5a),
                                Circling.Instance(7.5f, 25, 7, 0x0d59)
                            )
                        ),
                        SimpleWandering.Instance(5)
                    ),
                    Cooldown.Instance(1000, new RunBehaviors(
                        SimpleAttack.Instance(10, projectileIndex: 1),
                        MultiAttack.Instance(10, 5 * (float)Math.PI / 180, 2, projectileIndex: 0)
                    ))
                ));
    }
}
