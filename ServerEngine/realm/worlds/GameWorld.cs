﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerEngine.realm.setpieces;
using ServerEngine.realm.entities;

namespace ServerEngine.realm.worlds
{
    class GameWorld : World
    {
        public static GameWorld AutoName(int mapId, bool oryxPresent)
        {
            string name = RealmManager.realmNames[new Random().Next(RealmManager.realmNames.Count)];
            RealmManager.realmNames.Remove(name);
            return new GameWorld(mapId, name, oryxPresent);
        }
        public GameWorld(int mapId, string name, bool oryxPresent)
        {
            Name = name;
            Background = 0;
            base.FromWorldMap(typeof(RealmManager).Assembly.GetManifestResourceStream("ServerEngine.realm.worlds.world" + mapId + ".wmap"));
            SetPieces.ApplySetPieces(this);
            if (oryxPresent)
            {
                Overseer = new Oryx(this);
                Overseer.Init();
            }
            else
                Overseer = null;
        }

        public Oryx Overseer { get; private set; }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);
            if (Overseer != null)
                Overseer.Tick(time);
        }

        public void EnemyKilled(Enemy enemy, Player killer)
        {
            if (Overseer != null)
                Overseer.OnEnemyKilled(enemy, killer);
        }

        public override int EnterWorld(Entity entity)
        {
            var ret = base.EnterWorld(entity);
            if (entity is Player) //Make sure that AddWorld in RealmManager.cs has oryx present to TRUE
                Overseer.OnPlayerEntered(entity as Player);
            return ret;
        }
    }
}
