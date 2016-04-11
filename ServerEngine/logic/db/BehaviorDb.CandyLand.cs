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
        private static _ CandyLand = Behav()
            .Init(0x2229, Behaves("Gigacorn",
                SimpleWandering.Instance(1, .5f),
                    new RunBehaviors(
                        Cooldown.Instance(750, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 9, 1, projectileIndex: 0)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 4, 1, projectileIndex: 1))
                        )
                ));
    }
}