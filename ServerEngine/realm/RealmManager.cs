using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using System.Threading;
using System.Diagnostics;
using System.IO;
using ServerEngine.realm.worlds;
using System.Collections.Concurrent;
using ServerEngine.realm.entities;
using ServerEngine.realm;
using System.Globalization;
using ServerEngine.realm.commands;

namespace ServerEngine.realm
{
    public struct RealmTime //here all our stuff from LogicTicker.cs is jumbled into RealmTime
    {
        public long tickCount;
        public long tickTimes;
        public int thisTickCounts;
        public int thisTickTimes;
    }
    public class TimeEventArgs : EventArgs
    {
        public TimeEventArgs(RealmTime time)
        {
            this.Time = time;
        }
        public RealmTime Time { get; private set; }
    }

    public enum PendingPriority //More on this later
    {
        Emergent,
        Destruction,
        Networking,
        Normal,
        Creation,
    }

    public class RealmManager
    {
        public int TPS { get; private set; }
        public static List<string> realmNames = new List<string>()
        {
            "Elder", "Lord", "King"
        };

        static RealmManager()
        {
            //Worlds[World.TUT_ID] = new Tutorial(false);
            Worlds[World.NEXUS_ID] = Worlds[0] = new Nexus();
            Worlds[World.NEXUS_LIMBO] = new NexusLimbo();
            Worlds[World.VAULT_ID] = new Vault(true);
            Worlds[World.TEST_ID] = new Test();
            Worlds[World.RAND_REALM] = new RandomRealm();
            Worlds[World.GAUNTLET] = new GauntletMap();
            Worlds[World.WC] = new WineCellarMap();
            Worlds[World.ARENA] = new ArenaMap();
            Worlds[World.SHOP] = new ShopMap("Default");
            Worlds[World.MARKET] = new MarketMap();

            Monitor = new RealmPortalMonitor(Worlds[World.NEXUS_ID] as Nexus);

            AddWorld(GameWorld.AutoName(3, true));
            //AddWorld(GameWorld.AutoName(2, true)); //Comment out this line if you only want 1 realm (and less memory usage)

            MerchantLists.GetKeys();
            //MerchantLists.AddAdminShop();
            MerchantLists.AddCustomShops();
            foreach (var i in MerchantLists.shopLists)
            {
                ShopWorlds.TryAdd(i.Key, AddWorld(new ShopMap(i.Key)));
            }
        }

        public const int MAX_CLIENT = 50; //Only need 1-2 clients for private testing
        public static int nextWorldId = 0;
        public static int nextTestId = 0;
        public static readonly ConcurrentDictionary<int, World> Worlds = new ConcurrentDictionary<int, World>();
        public static readonly ConcurrentDictionary<int, Vault> Vaults = new ConcurrentDictionary<int, Vault>();
        public static readonly Dictionary<string, GuildHall> GuildHalls = new Dictionary<string, GuildHall>();
        public static readonly ConcurrentDictionary<int, ClientProcessor> Clients = new ConcurrentDictionary<int, ClientProcessor>();
        public static ConcurrentDictionary<int, World> PlayerWorldMapping = new ConcurrentDictionary<int, World>();

        public static ConcurrentDictionary<string, World> ShopWorlds = new ConcurrentDictionary<string, World>();

        public static RealmPortalMonitor Monitor { get; private set; }

        public static bool TryConnect(ClientProcessor psr)
        {
            var acc = psr.Account;
            if (psr.ConnectedBuild != psr.clientVer)
            {
                psr.SendPacket(new svrPackets.TextPacket()
                {
                    Name = "",
                    BubbleTime = 0,
                    Stars = -1,
                    Text = "Obtain an updated client from zeroeh.github.io"
                });
                return false;
            }
            if (acc.Banned)
                return false;
            if (Clients.Count >= MAX_CLIENT)
                return false;
            else
                return Clients.TryAdd(psr.Account.AccountId, psr);
        }
        public static void Disconnect(ClientProcessor psr)
        {
            Clients.TryRemove(psr.Account.AccountId, out psr);
        }

        public static Vault PlayerVault(ClientProcessor processor)
        {
            Vault v;
            int id = processor.Account.AccountId;
            if (Vaults.ContainsKey(id))
            {
                v = Vaults[id];
            }
            else
            {
                v = Vaults[id] = (Vault)AddWorld(new Vault(false, processor));
            }
            return v;
        }

        public static World GuildHallWorld(string g)
        {
            if (!GuildHalls.ContainsKey(g))
            {
                GuildHall gh = (GuildHall)AddWorld(new GuildHall(g));
                GuildHalls.Add(g, gh);
                return GuildHalls[g];
            }
            else
            {
                return GuildHalls[g];
            }
        }

        public static World AddWorld(World world)
        {
            world.Id = Interlocked.Increment(ref nextWorldId);
            Worlds[world.Id] = world;
            if (world is GameWorld)
                Monitor.WorldAdded(world);
            return world;
        }
        public static World GetWorld(int id)
        {
            World ret;
            if (!Worlds.TryGetValue(id, out ret)) return null;
            if (ret.Id == 0) return null;
            return ret;
        }
        public static List<Player> GuildMembersOf(string guild)
        {
            List<Player> members = new List<Player>();
            foreach (var i in Worlds)
            {
                if (i.Key != 0)
                {
                    foreach (var e in i.Value.Players)
                    {
                        if (e.Value.Guild.ToLower() == guild.ToLower())
                        {
                            members.Add(e.Value);
                        }
                    }
                }
            }
            return members;
        }
        public static Player FindPlayer(string name)
        {
            foreach (var i in Worlds)
            {
                if (i.Key != 0)
                {
                    foreach (var e in i.Value.Players)
                    {
                        if (e.Value.Client.Account.Name.ToLower() == name.ToLower())
                        {
                            return e.Value;
                        }
                    }
                }
            }
            return null;
        }
		//Start the network and logic threads
        static Thread network;
        public static NetworkTicker Network { get; private set; }

        static Thread logic;
        public static LogicTicker Logic { get; private set; }

        public XmlDatas GameData { get; private set; }
        public ChatManager Chat { get; private set; }

        public static void CoreTickLoop() //The core integrity that makes the server do its things
        {
            Network = new NetworkTicker();
            Logic = new LogicTicker();
            network = new Thread(Network.TickLoop)
            {
                Name = "Network Process Thread",
                CurrentCulture = CultureInfo.InvariantCulture
            };
            logic = new Thread(Logic.TickLoop)
            {
                Name = "Logic Ticking Thread",
                CurrentCulture = CultureInfo.InvariantCulture
            };
            logic.Start();
            network.Start();
            Thread.CurrentThread.Join(); //causes problems, may be a problem along with CoreTickLoop in program.cs
            GC.Collect(); //After threads are initialized, dispose any bad memory
        }
    }
}