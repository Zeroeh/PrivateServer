using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class SnakePit : World
    {
        public SnakePit()
        {
            Name = "Snake Pit";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.snakepit.wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new SnakePit());
        }
    }
}
