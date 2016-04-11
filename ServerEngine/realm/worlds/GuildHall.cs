using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerEngine.realm.worlds
{
    public class GuildHall : World
    {
        public string Guild { get; set; }
        
        public GuildHall(string guild)
        {
            Id = GHALL;
            Guild = guild;
            Name = "Guild Hall";
            Background = 0;
            AllowTeleport = true;
            switch (this.Level())
            {
                case 0:
                    base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.ghall0.wmap"));
                    break;
                case 1:
                    base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.ghall1.wmap"));
                    break;
                case 2:
                    base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.ghall2.wmap"));
                    break;
                case 3:
                    base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.ghall3.wmap"));
                    break;
            }
            //base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.guildhall0old.wmap"));            
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new GuildHall(Guild));
        }
        public int Level()
        {
            using (GameObjects.Database dbx = new GameObjects.Database())
            {
                int id = dbx.GetGuildId(this.Guild);
                return dbx.GetGuildLevel(id);
            }
        }
    }
}
