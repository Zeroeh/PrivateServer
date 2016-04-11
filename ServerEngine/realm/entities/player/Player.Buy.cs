using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;

namespace ServerEngine.realm.entities
{
    partial class Player
    {
        public void Buy(RealmTime time, BuyPacket pkt)
        {
            SellableObject obj = Owner.GetEntity(pkt.ObjectId) as SellableObject;
            if (obj != null)
                obj.Buy(this);
        }

        public void CheckCredits(RealmTime t, CheckCreditsPacket pkt)
        {
            psr.Database.ReadStats(psr.Account);
            Credits = psr.Account.Credits;
            UpdateCount++;
        }
    }
}
