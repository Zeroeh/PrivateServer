using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.svrPackets;
using ServerEngine.cliPackets;

namespace ServerEngine.realm.entities
{
    public partial class Player
    {
        long l = 0;
        long b = 0;
        bool OxygenRegen = false;
        void HandleGround(RealmTime time)
        {
            if (time.tickTimes - l > 500)
            {
                if (HasConditionEffect(ConditionEffects.Paused) ||
                    HasConditionEffect(ConditionEffects.Invincible))
                    return;

                try
                {
                    WmapTile tile = Owner.Map[(int)X, (int)Y];
                    ObjectDesc objDesc = tile.ObjType == 0 ? null : XmlDatas.ObjectDescs[tile.ObjType];
                    TileDesc tileDesc = XmlDatas.TileDescs[tile.TileId];
                    if (tileDesc.Damaging && (objDesc == null || !objDesc.ProtectFromGroundDamage))
                    {
                        int dmg = Random.Next(tileDesc.MinDamage, tileDesc.MaxDamage);
                        dmg = (int)statsMgr.GetDefenseDamage(dmg, true);
                            if (HP <= 0)
                            psr.Reconnect(new ReconnectPacket()
                            {
                                Host = "",
                                Port = 2050,
                                GameId = World.NEXUS_ID,
                                Name = "Nexus",
                                Key = Empty<byte>.Array,
                            });
                        HP -= dmg;
                        UpdateCount++;
                        l = time.tickTimes;
                    }
                }
                catch { }
            }
            if (time.tickTimes - b > 60)
            {
                try
                {
                    if (Owner.Name == "Ocean Trench")
                    {
                        bool fObject = false;
                        foreach (var i in Owner.StaticObjects)
                            if (i.Value.ObjectType == 0x0731)
                                if (Math.Floor(X) == Math.Floor(i.Value.X) && Math.Floor(Y) == Math.Floor(i.Value.Y))
                                    fObject = true;

                        //BEST COLLISION FORMULA EVER

                        if (fObject)
                            OxygenRegen = true;
                        else
                            OxygenRegen = false;

                        if (!OxygenRegen)
                        {
                            if (OxygenBar == 0)
                                HP -= 5;
                            else
                                OxygenBar -= 1;

                            if (HP <= 0)
                            psr.Reconnect(new ReconnectPacket()
                            {
                                Host = "",
                                Port = 2050,
                                GameId = World.NEXUS_ID,
                                Name = "Nexus",
                                Key = Empty<byte>.Array,
                            });
                            return;
                            //UpdateCount++; //Comment out
                        }
                        else
                        {
                            if (OxygenBar < 100)
                                OxygenBar += 15;
                            if (OxygenBar > 100)
                                OxygenBar = 100;

                            UpdateCount++;
                        }
                    }

                    b = time.tickTimes;
                }
                catch { }
            }
        }
        public void GroundDamage(RealmTime t, GroundDamagePacket pkt)
        {
        }
    }
}
