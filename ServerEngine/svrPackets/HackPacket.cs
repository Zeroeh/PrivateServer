using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;

namespace wServer.svrPackets
{
    public class HackPacket : ServerPacket
    {
        public int TickId { get; set; }
        public int TickTime { get; set; }
        public ObjectStats[] UpdateStatuses { get; set; }

        public override PacketID ID { get { return PacketID.Hack_Check; } }
        public override Packet CreateInstance() { return new HackPacket(); }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            TickId = rdr.ReadInt32();
            TickTime = rdr.ReadInt32();

            UpdateStatuses = new ObjectStats[rdr.ReadInt32()];
            for (var i = 0; i < UpdateStatuses.Length; i++)
                UpdateStatuses[i] = ObjectStats.Read(rdr);
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.Write(TickId);
            wtr.Write(TickTime);

            wtr.Write((short)UpdateStatuses.Length);
            foreach (var i in UpdateStatuses)
                i.Write(wtr);
        }
    }
}
