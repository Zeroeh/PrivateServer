﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.svrPackets;
using ServerEngine.realm.entities;

namespace ServerEngine.logic.taunt
{
    abstract class TauntBase : Behavior
    {
        protected void Taunt(string taunt, bool all)
        {
            if (taunt.Contains("{PLAYER}"))
            {
                float dist = 10;
                Entity player = GetNearestEntity(ref dist, null);
                if(player==null)return;
                taunt = taunt.Replace("{PLAYER}", player.Name);
            }
            taunt = taunt.Replace("{HP}", (Host as Enemy).HP.ToString());
            try
            {
                Host.Self.Owner.BroadcastPacket(new TextPacket()
                {
                    Name = "#" + (Host.Self.ObjectDesc.DisplayId ?? Host.Self.ObjectDesc.ObjectId),
                    ObjectId = Host.Self.Id,
                    Stars = -1,
                    BubbleTime = 5,
                    Recipient = "",
                    Text = taunt,
                    CleanText = ""
                }, null);
            }
            catch 
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("Crash halted - Nobody likes death...");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        protected abstract override bool TickCore(RealmTime time);
    }

    class SimpleTaunt : TauntBase
    {
        string taunt;
        public SimpleTaunt(string taunt) { this.taunt = taunt; }

        protected override bool TickCore(RealmTime time)
        {
            Taunt(taunt, false);
            return true;
        }
    }
}
