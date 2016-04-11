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
		static _ NexusBehavior = Behav()
			.Init(0x2239, Behaves("Nexus Flame A",
			new RunBehaviors(
					Circling.Instance(2, 20, 2, 0x2242)
				)))
			.Init(0x2240, Behaves("Nexus Flame B",
			new RunBehaviors(
					Circling.Instance(5, 20, 8, 0x2242)
				)))
			.Init(0x2241, Behaves("Nexus Flame C",
			new RunBehaviors(
					Circling.Instance(9, 20, 17, 0x2242)
				)));
    }
}
