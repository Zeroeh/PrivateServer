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
		static _ Shatters = Behav()
			.Init(0x2244, Behaves("The Crown",
			new RunBehaviors(
					new State("idle",
                        SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
						Cooldown.Instance(9500, RingAttack.Instance(12, 10, 12, projectileIndex: 0))
                    ),
					new State("transmute",
						new Transmute(0x2245)
					)),
					condBehaviors: new ConditionalBehavior[] {
                    new OnHit(new State("idle", SetState.Instance("transmute")))
                }
				))
			.Init(0x2245, Behaves("The Forgotten King",
				new RunBehaviors(
					new State("begin",
						Cooldown.Instance(10000, RingAttack.Instance(18, 10, 15, projectileIndex: 0))
						))
				));
    }
}