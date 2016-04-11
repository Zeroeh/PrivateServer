using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm.entities;
using ServerEngine.logic.loot;

namespace ServerEngine.realm.worlds
{
    public class MadLabMap : World
    {
        public MadLabMap()
        {
            Name = "Mad Lab";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.madlab.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new MadLabMap());
        }
    }
}
