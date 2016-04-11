using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.svrPackets
{
    public class WinPacket : ServerPacket
    {
        public int AccountId { get; set; }
        public int CharId { get; set; }

        public override PacketID ID { get { return PacketID.Win; } }
        public override Packet CreateInstance() { return new WinPacket(); }

        protected override void Read(ClientProcessor psr, NReader rdr)
        {
            AccountId = rdr.ReadInt32();
            CharId = rdr.ReadInt32();
        }

        protected override void Write(ClientProcessor psr, NWriter wtr)
        {
            wtr.Write(AccountId);
            wtr.Write(CharId);
        }
    }
}
