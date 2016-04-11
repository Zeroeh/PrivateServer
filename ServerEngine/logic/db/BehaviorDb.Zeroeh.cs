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
        private static _ Zeroeh = Behav()
            .Init(0x00592, Behaves("Zeroeh Wizard",
                MaintainDist.Instance(6, 15, 11, null),
                    new RunBehaviors(
                        Cooldown.Instance(900, PredictiveMultiAttack.Instance(35, 10 * (float)Math.PI / 180, 16, 1, projectileIndex: 0)),
                        Cooldown.Instance(200, PredictiveMultiAttack.Instance(35, 10 * (float)Math.PI / 180, 2, 1, projectileIndex: 1))
                        )
                ))
            .Init(0x00593, Behaves("Zeroeh Archer",
                SimpleWandering.Instance(1, .5f),
                    new RunBehaviors(
                        Cooldown.Instance(2000, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 1, 1, projectileIndex: 0)),
                        Cooldown.Instance(6500, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 1, 1, projectileIndex: 1)),
                        Cooldown.Instance(450, PredictiveMultiAttack.Instance(10, 10 * (float)Math.PI / 180, 6, 3, projectileIndex: 2))
                        )
                ))
            .Init(0x00594, Behaves("Zeroeh Ninja",
                Chasing.Instance(8, 10, 10, null)/*,
                    new RunBehaviors(
                        Cooldown.Instance(750, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 9, 1, projectileIndex: 0)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 4, 1, projectileIndex: 1))
                        )*/
                ));
    }
}