using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.cliPackets
{
    public abstract class ClientPacket : Packet
    {
        public override byte[] Crypt(ClientProcessor psr, byte[] dat, int len)
        {
            return psr.ReceiveKey.Crypt(dat, len);
        }
    }
}
