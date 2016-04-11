using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerEngine.realm.entities;
using ServerEngine.svrPackets;

namespace ServerEngine.realm.worlds
{
    public class Nexus : World
    {
        public Nexus()
        {
            Id = NEXUS_ID;
            Name = "Nexus";
            Background = 1;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.nexus.wmap"));
        }
    }
}
