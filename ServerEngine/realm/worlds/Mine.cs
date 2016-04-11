using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class Mine : World
    {
        public Mine()
        {
            Name = "Underground Mine";
            Background = 0;
            AllowTeleport = false;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.mine.wmap"));            
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new Mine());
        }
    }
}
