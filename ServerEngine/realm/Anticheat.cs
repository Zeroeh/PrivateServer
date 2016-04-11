using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using wServer.realm;
using System.Net.NetworkInformation;
using wServer.cliPackets;
using wServer.svrPackets;
using System.Reflection;
using System.Globalization;
using wServer.realm.commands;

namespace wServer
{
    private static void AutoBroadCastNews()
        {
            string[] news = File.ReadAllLines("./realm/hackcheck.txt");
            while (true)
            {
                foreach (var i in RealmManager.Clients.Values.ToArray())
                {
                    i.SendPacket(new HackPacket
                    {
                    
                    }
                }
                Thread.Sleep(120000);
            }
        }
}
