using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using log4net;

namespace ServerEngine.realm //All of this code is EXTREMELY buggy, this is basic foundation of the networking
{
    public class NetworkTicker //Sync network processing
    {
        ILog log = LogManager.GetLogger(typeof(NetworkTicker));
        public void AddPendingPacket(ClientProcessor client, Packet packet)
        {
            pendings.Enqueue(new Tuple<ClientProcessor, Packet>(client, packet));
            handle.Set();
        }
        AutoResetEvent handle = new AutoResetEvent(false);
        static ConcurrentQueue<Tuple<ClientProcessor, Packet>> pendings = new ConcurrentQueue<Tuple<ClientProcessor, Packet>>();
        public void TickLoop() //Sync ticks with client
        {
            do
            {
                foreach (var i in RealmManager.Clients) //every player
                    if (i.Value.Stage == ProtocalStage.Disconnected) //that is disonnected
                    {
                        ClientProcessor psr;
                        RealmManager.Clients.TryRemove(i.Key, out psr); //remove character from server
                    }
                handle.WaitOne();
                Tuple<ClientProcessor, Packet> work;
                while (pendings.TryDequeue(out work))
                {
                    if (work.Item1.Stage == ProtocalStage.Disconnected)
                    {
                        ClientProcessor psr;
                        RealmManager.Clients.TryRemove(work.Item1.Account.AccountId, out psr); //Disconnects clients on bad item sync (dupe)
                        continue;
                    }
                    try
                    {
                        work.Item1.ProcessPacket(work.Item2); //Tries to process the packet
                        //work.Item2(LogicTicker.CurrentTime); //was commented
                        /*
                        Perhaps work in LogicTicker, both that and NetworkTicker should work simultaneously
                        */
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error syncing the network, check NetworkTicker.cs");
                        log.Error(ex);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                //GC.Collect(); //Lags everything, not worth implementing unless done right
            } while (true);
        }
    }
}
