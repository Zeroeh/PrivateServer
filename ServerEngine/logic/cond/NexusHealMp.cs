using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.realm.entities;
using ServerEngine.svrPackets;

namespace ServerEngine.logic.cond
{
    class NexusHealMp : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            float dist = 10; //was 5
            Player entity = GetNearestEntity(ref dist, null) as Player;
            while (entity != null)
            {
                int mp = entity.MP;
                int maxMp = entity.Stats[1] + entity.Boost[1];
                mp = Math.Min(mp + 50, maxMp); //amount of mana restored
                if (mp != entity.MP)
                {
                    int n = mp - entity.MP;
                    entity.MP = mp;
                    entity.UpdateCount++;
                    entity.Owner.BroadcastPacket(new ShowEffectPacket()
                    {
                        EffectType = EffectType.Potion,
                        TargetId = entity.Id,
                        Color = new ARGB(0x0000FF) //was 0xffffffff as were the two below
                    }, null);
                    entity.Owner.BroadcastPacket(new ShowEffectPacket()
                    {
                        EffectType = EffectType.Trail,
                        TargetId = Host.Self.Id,
                        PosA = new Position() { X = entity.X, Y = entity.Y },
                        Color = new ARGB(0x000000)
                    }, null);
                    entity.Owner.BroadcastPacket(new NotificationPacket()
                    {
                        ObjectId = entity.Id,
                        Text = "+" + n,
                        Color = new ARGB(0x0000FF) //was 0xff9000ff
                    }, null);

                    return true;
                }
                entity = GetNearestEntity(ref dist, null) as Player;
            } 
            return false;
        }
    }
}
