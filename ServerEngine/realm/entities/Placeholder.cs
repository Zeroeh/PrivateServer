using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.svrPackets;
using ServerEngine.logic;

namespace ServerEngine.realm.entities
{
    class Placeholder : StaticObject
    {
        public Placeholder(int life)
            : base(0x070f, life, true, true, false)
        {
            Size = 0;
        }
    }
}
