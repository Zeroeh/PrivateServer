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
        static _ ShadowWell = Behav()
            .Init(0x2198, Behaves("Shaol, Eater of Souls",
                    new RunBehaviors(
                    new State("idle",
                        SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
                    ),
                    new State("begin", new QueuedBehavior(
                        new SimpleTaunt("..."),
                        CooldownExact.Instance(3000, NullBehavior.Instance),
                        new SimpleTaunt(".........."),
                        Timed.Instance(2000, False.Instance(Flashing.Instance(250, 0xffffff00))),
                        SetState.Instance("grow")
                    )),
                    new State("grow", SimpleWandering.Instance(5), new QueuedBehavior(
                        CooldownExact.Instance(1000, SetSize.Instance(120)),
                        CooldownExact.Instance(1000, SetSize.Instance(140)),
                        CooldownExact.Instance(1000, SetSize.Instance(160)),
                        CooldownExact.Instance(1000, SetSize.Instance(180)),
                        CooldownExact.Instance(1000, SetSize.Instance(200)),
                        new SimpleTaunt("Let us begin!"),
                        CooldownExact.Instance(1000, SetState.Instance("sacrifice"))
                    )),
                    new State("sacrifice",
                        StateOnce.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                        Once.Instance(new SimpleTaunt("I require a blood sacrifice!")),
                        Cooldown.Instance(2000, ThrowAttack.Instance(4, 6, 5000)),
                        HpLesserPercent.Instance((float)0.99, new RunBehaviors(
                            SetState.Instance("healing")))
                    ),
                    new State("healing",
                        StateOnce.Instance(new SimpleTaunt("You may as well try to kill a Shadow!")),
                        new QueuedBehavior(
                            Circling.Instance(10, 100, 5, 0x0d25),
                            Cooldown.Instance(3000, Heal.Instance(10, 25000, 0x2198)),
                            HpLesserPercent.Instance((float)0.80, new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            SetState.Instance("spawns")))
                    )),
                    new State("spawns",
                        StateOnce.Instance(new SimpleTaunt("I shall relish on the essence of your soul!")),
                        new QueuedBehavior(
                            Circling.Instance(10, 100, 5, 0x0d25),
                            Cooldown.Instance(1000, Heal.Instance(15, 10000, 0x2198)),
                            Once.Instance(new RunBehaviors(
                            SpawnMinionImmediate.Instance(0x00491, 5, 8, 10),
                            SpawnMinionImmediate.Instance(0x00490, 5, 7, 9),
                            HpLesserPercent.Instance((float)0.60, new RunBehaviors(
                            SetState.Instance("rageone")))
                    )))),
                    new State("rageone",
                        StateOnce.Instance(new SimpleTaunt("GRARGH!")),
                        new QueuedBehavior(
                            Chasing.Instance(6, 10, 0, null),
                            Cooldown.Instance(500, Heal.Instance(20, 5000, 0x2198)),
                            HpLesserPercent.Instance((float)0.40, new RunBehaviors(
                            SetState.Instance("etherphase")))
                    )),
                    new State("etherphase",
                        StateOnce.Instance(new SimpleTaunt("The Aether is quite fun. PERMANENT DELETION!")), //eather was Æther but doesn't work in client
                        new QueuedBehavior(
                            Circling.Instance(10, 100, 5, 0x0d25),
                            CooldownExact.Instance(2500, MultiAttack.Instance(10, 10 * (float)Math.PI / 180, 4, projectileIndex: 2)),
                            Cooldown.Instance(1500, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 10, 0, projectileIndex: 1)),
                            Cooldown.Instance(2000, RingAttack.Instance(5, 0, 5, projectileIndex: 0)),
                                Cooldown.Instance(100, AngleAttack.Instance(225)),
                                Cooldown.Instance(100, AngleAttack.Instance(36)),
                                Cooldown.Instance(100, AngleAttack.Instance(0)),
                                Cooldown.Instance(100, AngleAttack.Instance(135)),
                                Cooldown.Instance(100, AngleAttack.Instance(90))
                    ))
                ),
                loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 3, 0, 3,
                            Tuple.Create(0.01, (ILoot)new ItemLoot("Wine Cellar Incantation"))
                    ))),
                condBehaviors: new ConditionalBehavior[] {
                    new OnHit(new State("idle", SetState.Instance("begin")))
                }
                ));
    }
}
