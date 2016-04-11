using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerEngine.realm.entities;

namespace ServerEngine.realm.worlds
{
    public class RandomRealm : World
    {
        public RandomRealm()
        {
            Id = RAND_REALM;
            Name = "Random Realm";
            Background = 0;
            IsLimbo = true;
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.Monitor.GetRandomRealm();
        }
    }
}
