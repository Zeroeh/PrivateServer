﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class Island : World
    {
        public Island()
        {
            Name = "Forgotten Island";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.island.wmap"));            
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Island());
        }
    }
}
