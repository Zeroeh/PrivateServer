using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.svrPackets;
using System.Xml;
using System.Xml.Linq;
using ServerEngine.logic;

namespace ServerEngine.realm.entities
{
    public partial class Player
    {
        static int GetExpGoal(int level)
        {
            return 100 + (level - 1) * 125;
        }
        static int GetLevelExp(int level)
        {
            if (level == 1) return 0;
            return 40 * (level - 1) + (level - 2) * (level - 1) * 40;
        }
        static int GetFameGoal(int fame)
        {
            if (fame >= 1000) return 0;
            else if (fame >= 500) return 1000;
            else if (fame >= 300) return 500;
            else if (fame >= 150) return 300;
            else if (fame >= 20) return 150;
            else return 20;
        }

        public int GetStars()
        {
            int ret = 0;
            foreach (var i in psr.Account.Stats.ClassStates)
            {
                if (i.BestFame >= 1000) ret += 5;
                else if (i.BestFame >= 500) ret += 4;
                else if (i.BestFame >= 300) ret += 3;
                else if (i.BestFame >= 150) ret += 2;
                else if (i.BestFame >= 20) ret += 1;
            }
            return ret;
        }

        //float Dist(Entity a, Entity b)
        //{
        //    var dx = a.X - b.X;
        //    var dy = a.Y - b.Y;
        //    return (float)Math.Sqrt(dx * dx + dy * dy);
        //}

        void CalculateFame()
        {
            int newFame = 0;
            if (Experience < 200 * 1000) newFame = Experience / 1000;
            else newFame = 200 + (Experience - 200 * 1000) / 1000;
            if (newFame != Fame)
            {
                Fame = newFame;
                int newGoal;
                var state = psr.Account.Stats.ClassStates.SingleOrDefault(_ => _.ObjectType == ObjectType);
                if (state != null && state.BestFame > Fame)
                    newGoal = GetFameGoal(state.BestFame);
                else
                    newGoal = GetFameGoal(Fame);
                if (newGoal > FameGoal)
                {
                    Owner.BroadcastPacket(new NotificationPacket()
                    {
                        ObjectId = Id,
                        Color = new ARGB(0x00FF00),
                        Text = "Class Quest Complete!"
                    }, null);
                    Stars = GetStars();
                }
                FameGoal = newGoal;
                UpdateCount++;
            }
        }

        bool CheckLevelUp()
        {
            if (Experience - GetLevelExp(Level) >= ExperienceGoal && Level < 20) //change highest level
            {
                Level++;
                ExperienceGoal = GetExpGoal(Level);
                foreach (XElement i in XmlDatas.TypeToElement[ObjectType].Elements("LevelIncrease"))
                {
                    Random rand = new System.Random();
                    int min = int.Parse(i.Attribute("min").Value);
                    int max = int.Parse(i.Attribute("max").Value) + 1;
                    int limit = int.Parse(XmlDatas.TypeToElement[ObjectType].Element(i.Value).Attribute("max").Value);
                    int idx = StatsManager.StatsNameToIndex(i.Value);
                    Stats[idx] += rand.Next(min, max);
                    if (Stats[idx] > limit) Stats[idx] = limit;
                }
                HP = Stats[0] + Boost[0];
                MP = Stats[1] + Boost[1];

                UpdateCount++;

				if (Level == 20)
					foreach (var i in Owner.Players.Values)
						i.SendInfo(Name + " achieved level 20.");
				return true;
            }
            CalculateFame();
            return false;
        }

        //This last bit was commented because of quests
        public bool EnemyKilled(Enemy enemy, int exp, bool killer)
        {
            if (enemy == null)
                Owner.BroadcastPacket(new NotificationPacket()
                {
                    ObjectId = Id,
                    Color = new ARGB(0x00FF00),
                    Text = "Quest Complete!"
                }, null);
            if (exp > 0)
            {
                Experience += exp;
                UpdateCount++;
                foreach (var i in Owner.PlayersCollision.HitTest(X, Y, 16))
                {
                    if (i != (this as Entity))
                    {
                        try
                        {
                            (i as Player).Experience += exp;
                            (i as Player).UpdateCount++;
                            (i as Player).CheckLevelUp();
                        }
                        catch { }
                    }
                }
            }
            fames.Killed(enemy, killer);
            return CheckLevelUp();
        }
    }
}
