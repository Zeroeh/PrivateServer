using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class ForbiddenJungle : World
    {
		public ForbiddenJungle()
        {
            Name = "Forbidden Jungle";
            Background = 0;
            AllowTeleport = true;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.jungle.wmap"));            
        }

        public override World GetInstance(ClientProcessor psr)
        {
			return RealmManager.AddWorld(new ForbiddenJungle());
        }
    }
}
