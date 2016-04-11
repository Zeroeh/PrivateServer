using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.svrPackets;
using ServerEngine.realm.entities;

namespace ServerEngine.logic.taunt
{
    class ChefTaunt : ConditionalBehavior
    {
        public ChefTaunt(int t) { HPThreshold = t; }
        public int HPThreshold { get; set; }

        public override BehaviorCondition Condition { get { return BehaviorCondition.Other; } }

        protected override bool ConditionMeetCore()
        {
            float dist = 8;
            return GetNearestEntity(ref dist, null) != null || (Host as Character).HP < HPThreshold;
        }


        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            float dist = 8;

            int n = 0;
            object o;
            if (Host.StateStorage.TryGetValue(Key, out o))
                n = (int)o;
            else
                n = 0;

            if (n == 0 && GetNearestEntity(ref dist, null) != null)
            {
                Taunt("Time to test my power!!");
                Host.StateStorage[Key] = 1;
            }
            else if (n < 2 && (Host as Character).HP < HPThreshold)
            {
                Taunt("The admin weapons will never be yours!!");
                Host.StateStorage[Key] = 2;
            }
        }
    }
}
