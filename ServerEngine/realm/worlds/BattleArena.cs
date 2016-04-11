using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerEngine.svrPackets;
using ServerEngine.realm.entities;
using GameObjects;

namespace ServerEngine.realm.worlds
{
    public class BattleArenaMap : World
    {
        public int Wave = 0;
        public bool Waiting = false;

        public List<string> Participants = new List<string>();

        //public Dictionary<int, string[]> WaveEnemies = new Dictionary<int, string[]>();
        public string[] RandomizedEnemies;
        public string[] Bosses;

        public void InitWaveEnemies()
        {
            /*WaveEnemies.Add(1, new string[] { "Flying Brain", "Flying Brain", "Flying Brain", "Flying Brain", "Flying Brain" });
            WaveEnemies.Add(2, new string[] { "Flying Brain", "Flying Brain", "Medusa", "Medusa", "Lizard God", "Lizard God" });
            WaveEnemies.Add(3, new string[] { "Medusa", "Medusa", "Urgle", "Slime God", "Leviathan" });
            WaveEnemies.Add(4, new string[] { "Cube Overseer", "Cube Overseer", "Cube Overseer", "Leviathan", "Leviathan", "Slime God" });
            WaveEnemies.Add(5, new string[] { "Archdemon Malphas", "White Demon", "White Demon", "White Demon", "Malphas Missile", "Malphas Missile", "Leviathan" });
            WaveEnemies.Add(6, new string[] { "White Demon", "White Demon", "Leviathan", "Leviathan", "Urgle", "Urgle", "Ent Ancient" });
            WaveEnemies.Add(7, new string[] { "Ent Ancient", "Ent Ancient", "Ent Ancient", "Leviathan", "Rock Bot", "Urgle" });
            WaveEnemies.Add(8, new string[] { "Archdemon Malphas", "Rock Bot", "Rock Bot", "Leviathan", "Leviathan", "White Demon", "Urgle" });
            WaveEnemies.Add(9, new string[] { "Urgle", "Urgle", "Urgle", "Urgle", "Urgle", "Leviathan", "Rock Bot", "Rock Bot" });
            WaveEnemies.Add(10, new string[] { "Tomb Defender", "Archdemon Malphas", "Septavius the Ghost God", "Stheno the Snake Queen", "Coral Gift" });*/

            RandomizedEnemies = new string[] { 
                "Archdemon Malphas", "Stheno the Snake Queen", "Slime God", "Beholder", "Flying Brain", 
                "White Demon", "Henchman of Oryx", "Tomb Support", "Tomb Defender", "Lizard God", 
                "Thessal the Mermaid Goddess", "Phoenix Lord",
                "Red Demon", "Lizard God", "Beholder", "Slime God", "Flying Brain", "White Demon", "Beholder", 
                "Crystal Prisoner", "Davy Jones", "Oryx the Mad God 2", "Sprite God", "Sprite God", "Djinn", 
                "Hermit God", "Ghost Ship", "Tomb Attacker", "Septavius the Ghost God" };
        }

        public bool OutOfBounds(float x, float y)
        {
            return ((x <= 14.5f || x >= 40.5f) || (y <= 15.5f || y >= 45.5f));
        }

        public BattleArenaMap()
        {
            Name = "Battle Arena";
            Background = 0;
            AllowTeleport = true;
            Random r = new Random();
            InitWaveEnemies();
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.battlearena"+r.Next(1,4 +1).ToString()+".wmap"));
        }

        public override World GetInstance(ClientProcessor psr)
        {
            return RealmManager.AddWorld(new BattleArenaMap());
        }

        public void SpawnEnemies()
        {
            List<string> enems = new List<string>();
            Random r = new Random();
            for (int i = 0; i < Math.Ceiling((double)Wave/2); i++)
            {
                enems.Add(RandomizedEnemies[r.Next(0, RandomizedEnemies.Length)]);
            }
            Random r2 = new Random();
            foreach (string i in enems)
            {
                short id = XmlDatas.IdToType[i];
                float xloc = (float)r2.Next(15, 39) + 0.5f;
                float yloc = (float)r2.Next(16, 44) + 0.5f;
                Entity enemy = Entity.Resolve(id);
                enemy.Move(xloc, yloc);
                EnterWorld(enemy);
            }
        }

        public void Countdown(int s)
        {
            if (s == 5)
            {
                foreach (var i in Players)
                {
                    BroadcastPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xffff00ff),
                        ObjectId = i.Value.Id,
                        Text = "Wave " + Wave.ToString() + " - 5 Seconds"
                    }, null);
                    if (Wave % 21 == 0 && Wave != 0)
                    {
                        Container c = new Container(0x0509, 1000 * 120, true);
                        c.BagOwner = i.Value.AccountId;
                        c.Inventory[0] = XmlDatas.ItemDescs[0xc26];
                        c.Move(27.5f, 30.5f);
                        c.Size = 150;
                        EnterWorld(c);
                    }
                }
            }
            else if (s > 0)
            {
                foreach (var i in Players)
                {
                    BroadcastPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xffff00ff),
                        ObjectId = i.Value.Id,
                        Text = s.ToString() + " Second" + (s == 1 ? "" : "s")
                    }, null);
                }
            }
            else
            {
                SpawnEnemies();
            }
        }

        public void FullCountdown(int s)
        {
            Countdown(s);
            if (s == 0)
                Waiting = false;
            else
                Timers.Add(new WorldTimer(1000, (w, t) =>
                {
                    FullCountdown(s-1);
                }));
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if (Players.Count > 0)
            {
                if (Enemies.Count < 1) //if (Enemies.Count < 1 + Pets.Count)
                {
                    if (!Waiting)
                    {
                        Wave++;

                        Waiting = true;
                        ConditionEffect Invincible = new ConditionEffect();
                        Invincible.Effect = ConditionEffectIndex.Invulnerable;
                        Invincible.DurationMS = 5000;
                        ConditionEffect Healing = new ConditionEffect();
                        Healing.Effect = ConditionEffectIndex.Healing;
                        Healing.DurationMS = 5000;
                        foreach (var i in Players)
                        {
                            i.Value.ApplyConditionEffect(new ConditionEffect[]{
                                Invincible,
                                Healing
                            });
                        }
                        FullCountdown(5);
                        foreach (var i in Players)
                        {
                            try
                            {
                                if(!Participants.Contains(i.Value.Client.Account.Name))
                                    Participants.Add(i.Value.Client.Account.Name);
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    foreach (var i in Enemies)
                    {
                        if (OutOfBounds(i.Value.X, i.Value.Y))
                        {
                            LeaveWorld(i.Value);
                        }
                    }
                }
            }
            else
            {
                if (Participants.Count > 0)
                {
                    new Database().AddToArenaLB(Wave, Participants);
                    Participants.Clear();
                }
                foreach (var i in Enemies)
                {
                    LeaveWorld(i.Value);
                    Wave = 0;
                }
            }
        }
    }
}
