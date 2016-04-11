using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.realm
{
    public class WorldTimer
    {
        Action<World, RealmTime> cb;
        int remain;
        int total;

        int t = 0;
        public WorldTimer(int tickMs, Action<World, RealmTime> callback)
        {
            remain = total = tickMs;
            cb = callback;
            t = Environment.TickCount;
        }

        public void Reset()
        {
            remain = total;
        }

        public bool Tick(World world, RealmTime time)
        {
            remain -= time.thisTickTimes;
            if (remain < 1)
            {
                cb(world, time);
                return true;
            }
            return false;
        }
    }
}
