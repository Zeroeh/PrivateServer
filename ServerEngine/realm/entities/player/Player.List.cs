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
        public void SendAccountList(List<int> list, int id)
        {
            psr.SendPacket(new AccountListPacket()
            {
                AccountListId = id,
                AccountIds = list.ToArray()
            });
        }
    }
}
