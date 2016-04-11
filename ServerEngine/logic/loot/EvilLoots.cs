using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.logic.loot
{
    class EvilLoot : ILoot     //@ 0-eh
    {
        public Item GetLoot(Random rand)
        {
            return XmlDatas.ItemDescs[0x0a22];
        }
    }
}
