using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;
using ServerEngine.realm.setpieces;
using ServerEngine.realm.worlds;
using ServerEngine.realm.entities;
using ServerEngine.realm.entities.player;
using ServerEngine.logic.attack;
using TerrainEngine;
using GameObjects;

namespace ServerEngine.realm.commands
{
    class SpawnCommand : ICommand //NOTE: I have added loggers to all the admin commands. This will log anyone that attempts to use an admin command. Useful for tracking cheaters/hackers. Might slow down tile/texture loading because of all the file access. May become a problem with lots of mods/admins online in the server. If you need to, comment out the loggers.
    {
        public string Command { get { return "spawn"; } } //debating on making rank 1 or rank 2 command >_>
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            int num;
            if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                short objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, short> icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType) ||
                    !XmlDatas.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Error: Not known or bad spelling, try again.");
                }
                else
                {
                    int c = int.Parse(args[0]);
                    if (!(player.Client.Account.Rank > 1) && c > 5)
                    {
                        player.SendError("Spawn limit is 5 to prevent lag.");
                        return;
                    }
                    else if (player.Client.Account.Rank > 1 && c > 5)
                    {
                        player.SendInfo("Bypass made!");
                    }
                    for (int i = 0; i < num; i++)
                    {
                        var entity = Entity.Resolve(objType);
                        entity.Move(player.X, player.Y);
                        player.Owner.EnterWorld(entity);
                    }
                    player.SendInfo("Success!");
                }
            }
            else
            {
                string name = string.Join(" ", args);
                short objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, short> icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType) ||
                    !XmlDatas.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                }
                else
                {
                    var entity = Entity.Resolve(objType);
                    entity.Move(player.X, player.Y);
                    player.Owner.EnterWorld(entity);
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /spawn");
                }
            }
        }
    }

    class ArenaCommand : ICommand
    {
        public string Command { get { return "arena"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            var prtal = Portal.Resolve(0x1900);
            prtal.Move(player.X, player.Y);
            player.Owner.EnterWorld(prtal);
            World w = RealmManager.GetWorld(player.Owner.Id);
            w.Timers.Add(new WorldTimer(30 * 1000, (world, t) => //default portal close time * 1000
            {
                try
                {
                    w.LeaveWorld(prtal);
                }
                catch //couldn't remove portal, Owner became null. Should be fixed with RealmManager implementation
                {
                    Console.Out.WriteLine("Couldn't despawn portal.");
                }
            }));
            foreach (var i in RealmManager.Clients.Values)
                i.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Arena Opened by:" + " " + player.nName
                });
            foreach (var i in RealmManager.Clients.Values)
                i.SendPacket(new NotificationPacket()
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "Arena Opened by " + player.nName
                });
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /arena");
            }
        }
    }

    class AddEffCommand : ICommand
    {
        public string Command { get { return "buff"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /buff <Effectname or Effectnumber>");
            }
            else
            {
                try
                {
                    player.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                        DurationMS = -1
                    });
                    {
                        player.SendInfo("Success!");
                    }
                }
                catch
                {
                    player.SendError("Invalid effect!");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /buff");
                }
            }
        }
    }

    class RemoveEffCommand : ICommand
    {
        public string Command { get { return "debuff"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /debuff <Effectname or Effectnumber>");
            }
            else
            {
                try
                {
                    player.ApplyConditionEffect(new ConditionEffect()
                    {
                        Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                        DurationMS = 0
                    });
                    player.SendInfo("Success!");
                }
                catch
                {
                    player.SendError("Effect not known");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /debuff");
                }
            }
        }
    }

    class GiveCommand : ICommand
    {
        public string Command { get { return "gift"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /gift <Itemname>");
            }
            else
            {
                string name = string.Join(" ", args.ToArray()).Trim();
                short objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, short> icdatas = new Dictionary<string, short>(XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType))
                {
                    player.SendError("Error: No such item or object. Try retyping it.");
                    return;
                }
                if (!XmlDatas.ItemDescs[objType].Secret || player.Client.Account.Rank >= 3)
                {
                    for (int i = 0; i < player.Inventory.Length; i++)
                        if (player.Inventory[i] == null)
                        {
                            player.Inventory[i] = XmlDatas.ItemDescs[objType];
                            player.UpdateCount++;

                            player.SendInfo("Success!");
                            return;
                        }
                }
                else
                {
                    player.SendError("You are not an admin!");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /gift");
                }

            }
        }
    }

    class TpCommand : ICommand
    {
        public string Command { get { return "tp"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return;
                }
                player.Move(x + 0.5f, y + 0.5f);
                //player.SetNewbiePeriod();
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket()
                {
                    ObjectId = player.Id,
                    Position = new Position()
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /tp");
            }
        }
    }

    class SetpieceCommand : ICommand
    {
        public string Command { get { return "set"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /set <setpiece>");
            }
            else
            {
                try
                {
                    ISetPiece piece = (ISetPiece)Activator.CreateInstance(Type.GetType(
                        "ServerEngine.realm.setpieces." + args[0]));
                    piece.RenderSetPiece(player.Owner, new IntPoint((int)player.X + 1, (int)player.Y + 1));

                    player.SendInfo("Success!");
                }
                catch
                {
                    player.SendError("No such setpiece!");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /set");
                }
            }
        }
    }

    class DebugCommand : ICommand //Unknown/bugged command
    {
        public string Command { get { return "debug"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            player.Client.SendPacket(new TextBoxPacket
            {
                Button1 = "Okay",
                Button2 = "Nope",
                Message = "Are you ready for pain?",
                Title = "Watch Out!",
                Type = "Test"
            });
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /debug");
            }
        }
    }

    class KillAll : ICommand
    {
        public string Command { get { return "killall"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /killall <entityname>");
            }
            else
            {
                foreach (var i in player.Owner.Enemies)
                {
                    if ((i.Value.ObjectDesc != null) &&
                        (i.Value.ObjectDesc.ObjectId != null) &&
                        (i.Value.ObjectDesc.ObjectId.Contains(args[0])))
                    {
                        i.Value.Damage(player, new RealmTime(), 1000 * 10000, true); //may not work for ents/liches
                        //i.Value.Owner.LeaveWorld(i.Value);
                    }
                }
                player.SendInfo("Success!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /killall");
            }
        }
    }

    /*class KillAllX : ICommand //this version gives XP points, but does not work for enemies with evaluation/inv periods
    {
        public string Command { get { return "killallx"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /killallx <entityname>");
            }
            else
            {
                foreach (var i in player.Owner.Enemies)
                {
                    if ((i.Value.ObjectDesc != null) &&
                        (i.Value.ObjectDesc.ObjectId != null) &&
                        (i.Value.ObjectDesc.ObjectId.Contains(args[0])))
                    {
                        i.Value.Damage(player, new RealmTime(), 1000 * 10000, true); //may not work for ents/liches, 
                        //i.Value.Owner.LeaveWorld(i.Value);
                    }
                }
                player.SendInfo("Success!");
            }
        }
    }*/

    class Kick : ICommand
    {
        public string Command { get { return "kick"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <playername>");
            }
            else
            {
                try
                {
                    foreach (var i in player.Owner.Players)
                    {
                        if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                        {
                            player.SendInfo("Player Disconnected");
                            i.Value.Client.Disconnect();
                        }
                    }
                }
                catch
                {
                    player.SendError("Cannot kick!");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /kick");
                }
            }
        }
    }

    //class GetQuest : ICommand
    //{
    //    public string Command { get { return "getquest"; } }
    //    public int RequiredRank { get { return 2; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        player.SendInfo("Loc: " + player.Quest.X + " " + player.Quest.Y);
    //        var dir = @"logs";
    //        if (!System.IO.Directory.Exists(dir))
    //            System.IO.Directory.CreateDirectory(dir);
    //        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
    //        {
    //            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /getquest");
    //        }
    //    }
    //}

    class OryxSay : ICommand
    {
        public string Command { get { return "osay"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <saytext>");
            }
            else
            {
                string saytext = string.Join(" ", args);
                player.SendEnemy("Oryx the Mad God", saytext);
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /osay");
            }
        }
    }

    class SWhoCommand : ICommand //get all players from all worlds (this may become too large!)
    {
        public string Command { get { return "swho"; } }
        public int RequiredRank { get { return 0; } }

        public void Execute(Player player, string[] args)
        {
            StringBuilder sb = new StringBuilder("All online players: ");

            foreach (var w in RealmManager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    var copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                        }
                    }
                }
            }
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString);
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\CommandLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /swho");
            }
        }
    }

    class Announcement : ICommand
    {
        public string Command { get { return "announce"; } } //msg all players in all worlds
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
            }
            else
            {
                string saytext = string.Join(" ", args);

                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new TextPacket()
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "#Announcement",
                        Text = " " + saytext
                    });
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /announce");
            }
        }
    }

    class Summon : ICommand
    {
        public string Command { get { return "summon"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /summon <playername>");
            }
            else
            {
                foreach (var i in player.Owner.Players)
                {
                    if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Teleport(new RealmTime(), new cliPackets.TeleportPacket()
                        {
                            ObjectId = player.Id
                        });
                        return;
                    }
                }
                player.SendInfo(string.Format("Cannot summon, {0} not found!", args[0].Trim()));
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /summon");
            }
        }
    }

    //class KillCommand : ICommand
    //{
    //    public string Command { get { return "kill"; } }
    //    public int RequiredRank { get { return 2; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /kill <playername>");
    //        }
    //        else
    //        {
    //            foreach (var w in RealmManager.Worlds)
    //            {
    //                //string death = string.Join(" ", args);
    //                World world = w.Value;
    //                if (w.Key != 0) // 0 is limbo??
    //                {
    //                    foreach (var i in world.Players)
    //                    {
    //                        //Unnamed becomes a problem: skip them
    //                        if (i.Value.nName.ToLower() == args[0].ToLower().Trim() && i.Value.NameChosen)
    //                        {
    //                            i.Value.Death("Fate");

    //                            return;
    //                        }
    //                    }
    //                }
    //            }
    //            player.SendInfo(string.Format("Cannot kill, {0} not found!", args[0].Trim()));
    //        }
    //        var dir = @"logs";
    //        if (!System.IO.Directory.Exists(dir))
    //            System.IO.Directory.CreateDirectory(dir);
    //        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
    //        {
    //            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /kill");
    //        }
    //    }
    //}

    class RestartCommand : ICommand
    {
        public string Command { get { return "restart"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                foreach (var w in RealmManager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        world.BroadcastPacket(new TextPacket()
                        {
                            Name = "#Announcement",
                            Stars = -1,
                            BubbleTime = 0,
                            Text = "Server restarting to clear memory. Return in 2 minutes."
                        }, null);
                    }
                }

            }
            catch
            {
                player.SendError("Cannot say that in announcement!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /restart");
            }
        }
    }

    class VitalityCommand : ICommand
    {
        public string Command { get { return "vit"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /vit <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[5] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /vit");
            }
        }
    }

    class DefenseCommand : ICommand
    {
        public string Command { get { return "def"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /def <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[3] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /def");
            }
        }
    }

    class AttackCommand : ICommand
    {
        public string Command { get { return "atk"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /atk <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[2] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /atk");
            }
        }
    }

    class DexterityCommand : ICommand
    {
        public string Command { get { return "dex"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /dex <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[7] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /dex");
            }
        }
    }

    class LifeCommand : ICommand
    {
        public string Command { get { return "hp"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /hp <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[0] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /hp");
            }
        }
    }

    class ManaCommand : ICommand
    {
        public string Command { get { return "mp"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /mp <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[1] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /mp");
            }
        }
    }

    class SpeedCommand : ICommand
    {
        public string Command { get { return "spd"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /spd <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[4] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /spd");
            }
        }
    }

    class WisdomCommand : ICommand
    {
        public string Command { get { return "wis"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /wis <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[6] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /wis");
            }
        }
    }

    class Whitelist : ICommand
    {
        public string Command { get { return "whitelist"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /whitelist <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET rank=1 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not whitelist!");
                    }
                    else
                    {
                        player.SendInfo("Account successfully whitelisted!");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(player.nName + " has whitelisted " + args[0]);
                        Console.ForegroundColor = ConsoleColor.White;

                        var dir = @"logs";
                        if (!System.IO.Directory.Exists(dir))
                            System.IO.Directory.CreateDirectory(dir);
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\WhitelistLog.txt", true))
                        {
                            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " Has Whitelisted " + args[0]);
                        }
                    }
                }
            }
            catch
            {
                player.SendInfo("Server error. Please edit manually in database.");
            }
        }
    }

    class Ban : ICommand
    {
        public string Command { get { return "ban"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /ban <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not ban");
                    }
                    else
                    {
                        foreach (var i in player.Owner.Players)
                        {
                            if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                            {
                                i.Value.Client.Disconnect();
                                player.SendInfo("Account successfully banned");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Out.WriteLine(args[0] + " was banned.");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }
            }
            catch
            {
                player.SendInfo("Server error. Please edit manually in database.");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /ban");
            }
        }
    }

    class UnBan : ICommand
    {
        public string Command { get { return "unban"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unban <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=0, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not unban");
                    }
                    else
                    {
                        player.SendInfo("Account successfully unbanned");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(args[1] + " was unbanned.");
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                }
            }
            catch
            {
                player.SendInfo("Server error. Please edit manually in database.");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /unban");
            }
        }
    }

    class Rank : ICommand
    {
        public string Command { get { return "admin"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /admin <username> <number>\n0: Player\n1: Game Master\n2: Admin\n3: Project Leader");
            }
            else
            {
                try
                {
                    using (Database dbx = new Database())
                    {
                        var cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
                        cmd.Parameters.AddWithValue("@rank", args[1]);
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not change rank");
                        }
                        else
                            player.SendInfo("Account rank successfully changed");
                    }
                }
                catch
                {
                    player.SendInfo("Server error. Please edit manually in database.");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /admin");
                }
            }
        }
    }
    class GuildRank : ICommand
    {
        public string Command { get { return "grank"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /grank <username> <number>");
            }
            else
            {
                try
                {
                    using (Database dbx = new Database())
                    {
                        var cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
                        cmd.Parameters.AddWithValue("@guildRank", args[1]);
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not change guild rank. Use 10, 20, 30, 40, or 50 (invisible)");
                        }
                        else
                            player.SendInfo("Guild rank successfully changed");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(args[1] + "'s guild rank has been changed");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                catch
                {
                    player.SendInfo("Server error. Please edit manually in database.");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /grank");
                }
            }
        }
    }
    class ChangeGuild : ICommand
    {
        public string Command { get { return "setguild"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendHelp("Usage: /setguild <username> <guild id>");
            }
            else
            {
                try
                {
                    using (Database dbx = new Database())
                    {
                        var cmd = dbx.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET guild=@guild WHERE name=@name";
                        cmd.Parameters.AddWithValue("@guild", args[1]);
                        cmd.Parameters.AddWithValue("@name", args[0]);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            player.SendInfo("Could not change guild.");
                        }
                        else
                            player.SendInfo("Guild successfully changed");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(args[1] + "'s guild has been changed");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                catch
                {
                    player.SendInfo("Server error. Please edit manually in database.");
                }
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /setguild");
            }
        }
    }

    //class TqCommand : ICommand
    //{
    //    public string Command { get { return "tq"; } }
    //    public int RequiredRank { get { return 2; } }

    //    public void Execute(Player player, string[] args)
    //    {

    //        if (player.Quest == null)
    //        {
    //            player.SendError("Player does not have a quest!");
    //        }
    //        else
    //            player.Move(player.X + 0.5f, player.Y + 0.5f);
    //        //player.SetNewbiePeriod();
    //        player.UpdateCount++;
    //        player.Owner.BroadcastPacket(new GotoPacket() //In this lil bit of code, using the logs causes an un-noticable problem, doesn't affect or do anything, just putting this here just in case...
    //        {
    //            ObjectId = player.Id,
    //            Position = new Position()
    //            {
    //                X = player.Quest.X,
    //                Y = player.Quest.Y
    //            }
    //        }, null);
    //        player.SendInfo("Success!");
    //        var dir = @"logs";
    //        if (!System.IO.Directory.Exists(dir))
    //            System.IO.Directory.CreateDirectory(dir);
    //        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
    //        {
    //            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /tq");
    //        }
    //    }
    //}

    class GodMode : ICommand
    {
        public string Command { get { return "zen"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0
                });
                player.SendInfo("Zenmode Off");
            }
            else
            {

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.SendInfo("Zenmode On");
                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Immortality Granted"
                    });
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /zen");
            }
        }
    }

    //class GetIPCommand : ICommand
    //{
    //    public string Command { get { return "ip"; } }
    //    public int RequiredRank { get { return 2; } }

    //    public void Execute(Player player, string[] args)
    //    {
    //        var plr = player.Owner.GetUniqueNamedPlayerRough(string.Join(" ", args));
    //        if (plr != null)
    //        {
    //            player.SendInfo(plr.Name + "'s IP: " + plr.Client.IP.Address);
    //            return;
    //        }
    //        foreach (var i in RealmManager.Worlds)
    //        {
    //            if (i.Key != 0 && i.Value.Id != player.Owner.Id)
    //            {
    //                var p = i.Value.GetUniqueNamedPlayerRough(string.Join(" ", args));
    //                if (p != null)
    //                {
    //                    player.SendInfo(plr.Name + "'s IP: " + plr.Client.IP.Address);
    //                    return;
    //                }
    //                var dir = @"logs";
    //        if (!System.IO.Directory.Exists(dir))
    //            System.IO.Directory.CreateDirectory(dir);
    //        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
    //        {
    //            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /ip");
    //        }
    //            }
    //        }
    //        player.SendError("Could not find player.");
    //    }
    //}

    class VanishCommand : ICommand //Should make you COMPLETELY invisible to others.
    {
        public string Command { get { return "vanish"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            player.vanished = true;
            player.Owner.PlayersCollision.Remove(player);
            //if (player.Pet != null)
            {
                //player.Owner.LeaveWorld(player.Pet);
            }
        }
    }
    class StarCommand : ICommand
    {
        public string Command { get { return "stars"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /stars <ammount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stars = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /stars");
            }
        }
    }

    class LevelCommand : ICommand
    {
        public string Command { get { return "level"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /level <ammount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Character.Level = int.Parse(args[0]);
                    player.Client.Player.Level = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /level");
            }
        }
    }

    class NameCommand : ICommand
    {
        public string Command { get { return "name"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Use /name <name>");
            }
            else if (args.Length == 1)
            {
                using (Database db = new Database())
                {
                    var db1 = db.CreateQuery();
                    db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name;";
                    db1.Parameters.AddWithValue("@name", args[0]);
                    if ((int)(long)db1.ExecuteScalar() > 0)
                    {
                        player.SendError("Name Already In Use.");
                    }
                    else
                    {
                        db1 = db.CreateQuery();
                        db1.CommandText = "UPDATE accounts SET name=@name WHERE id=@accId";
                        db1.Parameters.AddWithValue("@name", args[0].ToString());
                        db1.Parameters.AddWithValue("@accId", player.Client.Account.AccountId.ToString());
                        if (db1.ExecuteNonQuery() > 0)
                        {
                            player.Client.Player.Credits = db.UpdateCredit(player.Client.Account, -0);
                            player.Client.Player.Name = args[0];
                            player.Client.Player.NameChosen = true;
                            player.Client.Player.UpdateCount++;
                            player.SendInfo("Success!");
                        }
                        else
                        {
                            player.SendError("Server error. Please edit manually in database.");
                        }
                    }
                }
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /name");
            }
        }
    }

    class RenameCommand : ICommand
    {
        public string Command { get { return "rename"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Use /rename <OldPlayerName> <NewPlayerName>");
            }
            else if (args.Length == 2)
            {
                using (Database db = new Database())
                {
                    var db1 = db.CreateQuery();
                    db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name;";
                    db1.Parameters.AddWithValue("@name", args[1]);
                    if ((int)(long)db1.ExecuteScalar() > 0)
                    {
                        player.SendError("Name Already In Use.");
                    }
                    else
                    {
                        db1 = db.CreateQuery();
                        db1.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name";
                        db1.Parameters.AddWithValue("@name", args[0]);
                        if ((int)(long)db1.ExecuteScalar() < 1)
                        {
                            player.SendError("Name Not Found.");
                        }
                        else
                        {
                            db1 = db.CreateQuery();
                            db1.CommandText = "UPDATE accounts SET name=@newName, namechosen=TRUE WHERE name=@oldName;";
                            db1.Parameters.AddWithValue("@newName", args[1]);
                            db1.Parameters.AddWithValue("@oldName", args[0]);
                            if (db1.ExecuteNonQuery() > 0)
                            {
                                foreach (var playerX in RealmManager.Worlds)
                                {
                                    if (playerX.Key != 0)
                                    {
                                        World world = playerX.Value;
                                        foreach (var p in world.Players)
                                        {
                                            Player Client = p.Value;
                                            if ((player.Name.ToLower() == args[0].ToLower()) && player.NameChosen)
                                            {
                                                player.Name = args[1];
                                                player.NameChosen = true;
                                                player.UpdateCount++;
                                                break;
                                            }
                                        }
                                    }
                                }
                                player.SendInfo("Success!");
                            }
                            else
                            {
                                player.SendError("Server error. Please edit manually in database.");
                            }
                        }
                    }
                }
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /rename");
            }
        }
    }
    /*
    class giveEffCommand : ICommand
    {
        public string Command { get { return "giftbuff"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /giftbuff <Effectname or Effectnumber> <Playername>");
            }
            else
            {
                try
                {
                    var n = "";
                    foreach (var i in RealmManager.Clients.Values)
                    {
                        if (i.Account.Name.ToUpper().StartsWith("[Lord]"))
                        {
                            n = i.Account.Name.Substring(3);
                        }

                        if (n.ToUpper() == args[0].ToUpper())
                        {
                            ConditionEffectIndex cond = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[1].Trim());
                            //check if the player already has the condition effect
                            i.Player.ApplyConditionEffect(new ConditionEffect()
                            {
                                Effect = cond,
                                DurationMS = -1
                            });
                            {
                                player.SendInfo("Success!");
                            }
                        }
                    }
                }
                catch
                {
                    player.SendError("Invalid effect!");
                }
            }
        }
    }
    */
    class messageCommand : ICommand
    {
        public string Command { get { return "message"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /message <title> <message>");
            }
            else
            {
                string title = string.Join(" ", args);
                string message = string.Join(" ", args.Skip(1).ToArray());
                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new TextBoxPacket()
                    {
                        Title = args[0],
                        Message = message,
                        Button1 = "Ok",
                        Type = "GlobalMsg"
                    });
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /message");
                }
            }
        }
    }
    class SumCommand : ICommand //Checks clients for hacks, then bans the offender.
    {
        public string Command { get { return "checksum"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /checksum <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=1 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not perform checksum!");
                    }
                    else
                    {
                        player.SendInfo("Account successfully checksummed!");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(player.nName + " Has checksummed " + args[0]);
                        Console.ForegroundColor = ConsoleColor.White;

                        var dir = @"logs";
                        if (!System.IO.Directory.Exists(dir))
                            System.IO.Directory.CreateDirectory(dir);
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\CheckSumLog.txt", true))
                        {
                            writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has Checksummed " + args[0]);
                        }
                    }
                }
            }
            catch
            {
                player.SendInfo("Could not checksum!");
            }
        }
        class VisitCommand : ICommand
        {
            public string Command { get { return "goto"; } }
            public int RequiredRank { get { return 2; } }

            public bool TryJoin(Player player, GlobalPlayerData iPlayerData, World world, Player i)
            {
                if (!iPlayerData.Solo)
                {
                    if (!iPlayerData.UsingGroup)
                    {
                        player.Client.Reconnect(new ReconnectPacket()
                        {
                            Host = "",
                            Port = 2050,
                            GameId = world.Id,
                            Name = world.Name,
                            Key = Empty<byte>.Array,
                        });
                        return true;
                    }
                    else
                    {
                        foreach (var o in iPlayerData.JGroup)
                        {
                            if (o.Value == player.Client.Account.Name.ToLower())
                            {
                                player.Client.Reconnect(new ReconnectPacket()
                                {
                                    Host = "",
                                    Port = 2050,
                                    GameId = world.Id,
                                    Name = world.Name,
                                    Key = Empty<byte>.Array,
                                });
                                return true;
                            }
                        }
                        player.SendInfo("Not in " + i.Client.Account.Name + "'s group!");
                        return true;
                    }
                }
                else
                {
                    player.SendInfo("Player is going solo!");
                    return true;
                }
            }

            public void Execute(Player player, string[] args)
            {
                string name = string.Join(" ", args.ToArray()).Trim();
                try
                {
                    GlobalPlayerData PlayerData = PlayerDataList.GetData(player.Client.Account.Name);
                    foreach (var w in RealmManager.Worlds)
                    {
                        World world = w.Value;
                        if (w.Key != 0)
                        {
                            foreach (var i in world.Players)
                            {
                                if (i.Value.Client.Account.Name.ToLower() == name.ToLower())
                                {
                                    GlobalPlayerData iPlayerData = PlayerDataList.GetData(i.Value.Client.Account.Name);
                                    if (!(player.Client.Account.Rank > 2))
                                    {
                                        if (world.Name != "Vault")
                                        {
                                            if (world.Name != "Guild Hall")
                                            {
                                                TryJoin(player, iPlayerData, world, i.Value); return;
                                            }
                                            else
                                            {
                                                if ((world as GuildHall).Guild == player.Guild)
                                                {
                                                    TryJoin(player, iPlayerData, world, i.Value); return;
                                                }
                                                else
                                                {
                                                    player.SendInfo("Player is in " + i.Value.Guild + "'s guild hall!");
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (world.Name == "Vault")
                                            {
                                                player.SendInfo("Player is in Vault!");
                                                return;
                                            }
                                            else if (world.Name == "Guild Hall")
                                            {
                                                player.SendInfo("Player is in Guild Hall!");
                                                return;
                                            }
                                            else
                                            {
                                                if (!iPlayerData.UsingGroup)
                                                {
                                                    player.Client.Reconnect(new ReconnectPacket()
                                                    {
                                                        Host = "",
                                                        Port = 2050,
                                                        GameId = world.Id,
                                                        Name = i.Value.Name + "'s Vault",
                                                        Key = Empty<byte>.Array,
                                                    });
                                                    return;
                                                }
                                                else
                                                {
                                                    foreach (var o in iPlayerData.JGroup)
                                                    {
                                                        if (o.Value == player.Client.Account.Name.ToLower())
                                                        {
                                                            player.Client.Reconnect(new ReconnectPacket()
                                                            {
                                                                Host = "",
                                                                Port = 2050,
                                                                GameId = world.Id,
                                                                Name = i.Value.Name + "'s Vault",
                                                                Key = Empty<byte>.Array,
                                                            });
                                                            return;
                                                        }
                                                    }
                                                    player.SendInfo("Not in " + i.Value.Client.Account.Name + "'s group!");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        player.Client.Reconnect(new ReconnectPacket()
                                        {
                                            Host = "",
                                            Port = 2050,
                                            GameId = world.Id,
                                            Name = i.Value.Owner.Name,
                                            Key = Empty<byte>.Array,
                                        });
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    player.SendHelp("Use /goto <playername>");
                }
                catch
                {
                    player.SendInfo("Unexpected error in command!");
                }
                var dir = @"logs";
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /goto");
                }
            }
        }

    }
    class Mute : ICommand
    {
        public string Command { get { return "mute"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /mute <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET muted=1, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not mute.");
                    }
                    else
                    {
                        foreach (var i in player.Owner.Players)
                        {
                            if (i.Value.nName.ToLower() == args[0].ToLower().Trim())
                            {
                                i.Value.Client.Disconnect();
                                player.SendInfo("Account successfully muted.");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Out.WriteLine(args[0] + " was muted.");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }
            }
            catch
            {
                player.SendInfo("Server error. Please edit manually in database.");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /mute");
            }
        }
    }

    class UnMute : ICommand
    {
        public string Command { get { return "unmute"; } }
        public int RequiredRank { get { return 1; } }

        public void Execute(Player player, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unmute <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET muted=0, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not unmute.");
                    }
                    else
                    {
                        player.SendInfo("Account successfully unmuted");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.WriteLine(args[1] + " was unmuted.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch
            {
                player.SendInfo("Server error. Please edit manually in database.");
            }
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /unmute");
            }
        }
    }
    /*class ResizeCommand : ICommand
    {
        public string Command { get { return "size"; } }
        public int RequiredRank { get { return 2; } }

        public void Execute(Player player, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /size <amount>");
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stats[9] = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
            }
        }
    }*/
    internal class Unvanish : ICommand
    {
        public string Command
        {
            get { return "unvanish"; }
        }

        public int RequiredRank
        {
            get { return 3; }
        }

        public void Execute(Player player, string[] args)
        {
            player.vanished = false;
        }
    }
    class Spectate : ICommand
    {
        public string Command { get { return "spectate"; } }
        public int RequiredRank { get { return 3; } }

        public void Execute(Player player, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible) && (player.HasConditionEffect(ConditionEffects.Invisible)))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = 0
                });
                player.SendInfo("Spectate Off");
            }
            else
            {

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = -1
                });
                player.SendInfo("Spectate On");
                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Spectate Activated"
                    });
            }
        }
    }
    class AdminBuff : ICommand
    {
        public string Command
        {
            get
            {
                return "adminbuff";
            }
        }
        public int RequiredRank
        {
            get
            {
                return 2;
            }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Berserk) && (player.HasConditionEffect(ConditionEffects.Damaging) && (player.HasConditionEffect(ConditionEffects.Armored) && (player.HasConditionEffect(ConditionEffects.Healing)))))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Berserk,
                    DurationMS = 0
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Damaging,
                    DurationMS = 0
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Armored,
                    DurationMS = 0
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Healing,
                    DurationMS = 0
                });
                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Admin Buff Disabled"
                    });
            }
            else
            {

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Berserk,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Damaging,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Armored,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Healing,
                    DurationMS = -1
                });
                player.SendInfo("Admin Buff Enabled");
                foreach (var i in RealmManager.Clients.Values)
                    i.SendPacket(new NotificationPacket()
                    {
                        Color = new ARGB(0xff00ff00),
                        ObjectId = player.Id,
                        Text = "Admin Buff Enabled"
                    });
            }
        }
    }
    class Relay : ICommand
    {
        public string Command
        {
            get
            {
                return "krelay";
            }
        }
        public int RequiredRank
        {
            get
            {
                return 3;
            }
        }
        public void Execute(Player player, string[] args)
        {
            foreach (var i in RealmManager.Clients.Values)
            i.SendPacket(new NotificationPacket()
            {
                Color = new ARGB(0x00FFFF),
                ObjectId = player.Id,
                Text = "K Relay Enabled"
            });
            var dir = @"logs";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\AdminLog.txt", true))
            {
                writer.WriteLine("[" + DateTime.Now + "]" + player.nName + " has used the /krelay");
            }
        }
    }
}
