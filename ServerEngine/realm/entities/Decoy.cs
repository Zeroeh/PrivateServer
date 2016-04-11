﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.svrPackets;
using Mono.Game;
using ServerEngine.logic;

namespace ServerEngine.realm.entities
{
    class Decoy : StaticObject, IPlayer
    {
        static Random rand = new Random();

        Player player;
        int duration;
        Vector2 direction;
        float speed;

        Vector2 GetRandDirection() //Well....
        {
            double angle = rand.NextDouble() * 1 * Math.PI;
            return new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            );
        }
        public Decoy(Player player, int duration, float tps)
            : base(0x0715, duration, true, true, true)
        {
            this.player = player;
            this.duration = duration;
            this.speed = tps;

            var history = player.TryGetHistory(100);
            if (history == null)
                direction = GetRandDirection();
            else
            {
                direction = new Vector2(player.X - history.Value.X, player.Y - history.Value.Y);
                if (direction.LengthSquared() == 0)
                    direction = GetRandDirection();
                else
                    direction.Normalize();
            }
        }

        public static Decoy DecoyRandom(Player player, int duration, float tps)
        {
            Decoy d = new Decoy(player, duration, tps);
            d.direction = d.GetRandDirection();
            return d;
        }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.Texture1] = player.Texture1;
            stats[StatsType.Texture2] = player.Texture2;
            base.ExportStats(stats);
        }

        bool exploded = false;
        public override void Tick(RealmTime time)
        {
            if (HP > duration / 2)
            {
                Behavior.ValidateAndMove(this,
                    X + direction.X * speed * time.thisTickTimes / 800, //They were 1900
                    Y + direction.Y * speed * time.thisTickTimes / 800
                );
            }
            if (HP < 250 && !exploded)
            {
                exploded = true;
                Owner.BroadcastPacket(new ShowEffectPacket()
                {
                    EffectType = EffectType.AreaBlast,
                    Color = new ARGB(0xffff0000),
                    TargetId = Id,
                    PosA = new Position() { X = 1 }
                }, null);
            }
            base.Tick(time);
        }

        public void Damage(int dmg, Character chr) { }

        public bool IsVisibleToEnemy() { return true; }
    }
}
