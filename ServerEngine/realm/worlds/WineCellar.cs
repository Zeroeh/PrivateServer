﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class WineCellarMap : World
    {
        public WineCellarMap()
        {
            Id = WC;
            Name = "Wine Cellar";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.winecellar.wmap"));            
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new WineCellarMap());
        }
    }
}
