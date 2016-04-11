using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.realm.entities;
using ServerEngine.svrPackets;

namespace ServerEngine.logic.cond
{
    class NexusHealHp : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            float dist = 10; //was 5
            Player entity = GetNearestEntity(ref dist, null) as Player;
            while (entity != null)
            {
                int hp = entity.HP;
                int maxHp = entity.Stats[0] + entity.Boost[0];
                hp = Math.Min(hp + 100, maxHp); //max health healed
                if (hp != entity.HP)
                {
                    int n = hp - entity.HP;
                    entity.HP = hp;
                    entity.UpdateCount++;
                    entity.Owner.BroadcastPacket(new ShowEffectPacket()
                    {
                        EffectType = EffectType.Potion,
                        TargetId = entity.Id,
                        Color = new ARGB(0xFF0000) //was 0xffffffff as was below
                    }, null);
                    entity.Owner.BroadcastPacket(new ShowEffectPacket()
                    {
                        EffectType = EffectType.Trail,
                        TargetId = Host.Self.Id,
                        PosA = new Position() { X = entity.X, Y = entity.Y },
                        Color = new ARGB(0x000000) //six zero is black. six f is white. Look up colors in html
                    }, null);
                    entity.Owner.BroadcastPacket(new NotificationPacket()
                    {
                        ObjectId = entity.Id,
                        Text = "+" + n,
                        Color = new ARGB(0xFF0000) //was 0xff00ff00
                    }, null);

                    return true;
                }
                entity = GetNearestEntity(ref dist, null) as Player;
            } 
            return false;
        }
    }
}
