using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;
using System.Net.Sockets;
using System.Net;

namespace ServerEngine //Base code for writting packets, regarding data and encryption
{
    public abstract class Packet //Another base foundation for the networking, fairly decent, but could use work
    {
        public static Dictionary<PacketID, Packet> Packets = new Dictionary<PacketID, Packet>();
        static Packet()
        {
            foreach (var i in typeof(Packet).Assembly.GetTypes())
                if (typeof(Packet).IsAssignableFrom(i) && !i.IsAbstract)
                {
                    Packet pkt = (Packet)Activator.CreateInstance(i);
                    if (!(pkt is ServerPacket))
                        Packets.Add(pkt.ID, pkt);
                }
        }
        public abstract PacketID ID { get; } //get our packet ID
        public abstract Packet CreateInstance(); //start creating our packet

        public abstract byte[] Crypt(ClientProcessor psr, byte[] dat, int len); //crypt it

        public void Read(ClientProcessor psr, byte[] body, int len) //packet is sent to our virtual client (clientprocessor)
        {
            Read(psr, new NReader(new MemoryStream(Crypt(psr, body, len)))); //packet is read into the memory
        }
        public byte[] Write(ClientProcessor psr) //packet is taken from the memory and written
        {
            MemoryStream s = new MemoryStream();
            this.Write(psr, new NWriter(s));

            byte[] content = s.ToArray();
            byte[] ret = new byte[5 + content.Length];
            content = this.Crypt(psr, content, content.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ret.Length)), 0, ret, 0, 4);
            ret[4] = (byte)this.ID;
            Buffer.BlockCopy(content, 0, ret, 5, content.Length);
            return ret;
        }

        protected abstract void Read(ClientProcessor psr, NReader rdr);
        protected abstract void Write(ClientProcessor psr, NWriter wtr);

        public override string ToString() //packet has its variables appended
        {
            StringBuilder ret = new StringBuilder("{");
            var arr = GetType().GetProperties();
            for (var i = 0; i < arr.Length; i++)
            {
                if (i != 0) ret.Append(", ");
                ret.AppendFormat("{0}: {1}", arr[i].Name, arr[i].GetValue(this, null));
            }
            ret.Append("}");
            return ret.ToString();
        }
    }

    public class NopPacket : Packet //NopPacket a.k.a. holy grail packet
    {
        public override PacketID ID { get { return PacketID.UpdateAck; } } //start by telling the server/client of an update
        public override Packet CreateInstance() { return new NopPacket(); } //create our noppacket
        public override byte[] Crypt(ClientProcessor psr, byte[] dat, int len) { return dat; } //packet is processed (crypt/decrypt)
        protected override void Read(ClientProcessor psr, NReader rdr) { } //contents are read
        protected override void Write(ClientProcessor psr, NWriter wtr) { } //new packet is written
    }
}
