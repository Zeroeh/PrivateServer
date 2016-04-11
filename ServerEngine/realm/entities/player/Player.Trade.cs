using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;
using System.Collections.Concurrent;

namespace ServerEngine.realm.entities
{
    partial class Player
    {
        Dictionary<Player, int> potentialTrader = new Dictionary<Player, int>();
        Player tradeTarget;
        bool[] trade;
        bool tradeAccepted;
        private int itemnumber1; //Start of unknown code
        private int itemnumber2;
        public static string items1 { get; set; }
        public static string items2 { get; set; } //End of unknown code

        public void RequestTrade(RealmTime time, RequestTradePacket pkt)
        {
            if (!NameChosen)
            {
                SendInfo("Unique name is required to trade with others!");
                return;
            }
            if (tradeTarget != null)
            {
                SendError("You're already trading!");
                tradeTarget = null;
                return;
            }
            Player target = Owner.GetUniqueNamedPlayer(pkt.Name);
            if (target == null)
            {
                SendError("Player not found or is offline!");
                return;
            }
            if (target.tradeTarget != null && target.tradeTarget != this)
            {
                SendError(target.Name + " is already trading!");
                return;
            }

            if (this.potentialTrader.ContainsKey(target))
            {
                this.tradeTarget = target;
                this.trade = new bool[12];
                this.tradeAccepted = false;
                target.tradeTarget = this;
                target.trade = new bool[12];
                target.tradeAccepted = false;
                this.potentialTrader.Clear();
                target.potentialTrader.Clear();

                TradeItem[] my = new TradeItem[Inventory.Length];
                for (int i = 0; i < Inventory.Length; i++)
                    my[i] = new TradeItem()
                    {
                        Item = this.Inventory[i] == null ? -1 : Inventory[i].ObjectType,
                        SlotType = this.SlotTypes[i],
                        Included = false,
                        Tradeable = (Inventory[i] == null || i < 4) ? false : (!Inventory[i].Soulbound && !Inventory[i].Undead && !Inventory[i].SUndead)
                    };
                TradeItem[] your = new TradeItem[target.Inventory.Length];
                for (int i = 0; i < target.Inventory.Length; i++)
                    your[i] = new TradeItem()
                    {
                        Item = target.Inventory[i] == null ? -1 : target.Inventory[i].ObjectType,
                        SlotType = target.SlotTypes[i],
                        Included = false,
                        Tradeable = (target.Inventory[i] == null || i < 4) ? false : (!target.Inventory[i].Soulbound && !target.Inventory[i].Undead && !target.Inventory[i].SUndead)
                    };

                psr.SendPacket(new TradeStartPacket()
                {
                    MyItems = my,
                    YourName = target.Name,
                    YourItems = your
                });
                target.psr.SendPacket(new TradeStartPacket()
                {
                    MyItems = your,
                    YourName = this.Name,
                    YourItems = my
                });
            }
            else
            {
                target.potentialTrader[this] = 1000 * 20;
                target.psr.SendPacket(new TradeRequestedPacket()
                {
                    Name = Name
                });
                SendInfo("You have requested to trade with " + target.Name);
                return;
            }
        }
        public void ChangeTrade(RealmTime time, ChangeTradePacket pkt)
        {
            if (this.trade != pkt.Offers)
            {
                this.tradeAccepted = false;
                tradeTarget.tradeAccepted = false;
                this.trade = pkt.Offers;

                tradeTarget.psr.SendPacket(new TradeChangedPacket()
                {
                    Offers = this.trade
                });
            }
        }
        public void AcceptTrade(RealmTime time, AcceptTradePacket pkt)
        {
            this.trade = pkt.MyOffers;
            if (tradeTarget.trade.SequenceEqual(pkt.YourOffers))
            {
                tradeTarget.trade = pkt.YourOffers;
                this.tradeAccepted = true;
                tradeTarget.psr.SendPacket(new TradeAcceptedPacket()
                {
                    MyOffers = tradeTarget.trade,
                    YourOffers = this.trade
                });
                //Console.Out.WriteLine("Player {0} accepted trade with {1}", nName, tradeTarget.nName); //this line is duplicated in line 132
                var dir = @"logs"; //start of logging code
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                //Console.WriteLine("[3]0 has fully traded with 1", nName, tradeTarget.nName, DateTime.Now);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\TradeLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + nName + "has finalized the trade with " + tradeTarget.nName);
                } //end of logging code
                /*if (this.tradeAccepted && tradeTarget.tradeAccepted)
                {
                    DoTrade();
                    Console.Out.WriteLine("Did trade!");
                }*/
            }
        }
        public void CancelTrade(RealmTime time, CancelTradePacket pkt)
        {
            this.psr.SendPacket(new TradeDonePacket()
            {
                Result = 1,
                Message = "Trade cancelled."
            });
            tradeTarget.psr.SendPacket(new TradeDonePacket()
            {
                Result = 1,
                Message = "Trade cancelled."
            });

            tradeTarget.tradeTarget = null;
            tradeTarget.trade = null;
            tradeTarget.tradeAccepted = false;
            this.tradeTarget = null;
            this.trade = null;
            this.tradeAccepted = false;
            return;
        }
        void TradeTick(RealmTime time)
        {
            if (trade != null)
                if (tradeTarget != null)
                    if (tradeAccepted && tradeTarget.tradeAccepted)
                        DoTrade();
            CheckTradeTimeout(time);
        }
        void CheckTradeTimeout(RealmTime time)
        {
            List<Tuple<Player, int>> newState = new List<Tuple<Player, int>>();
            foreach (var i in potentialTrader)
                newState.Add(new Tuple<Player, int>(i.Key, i.Value - time.thisTickTimes));

            foreach (var i in newState)
            {
                if (i.Item2 < 0)
                {
                    {
                        i.Item1.SendError("Your trade request has timed out!"); //i.Item1.SendError("Trade to " + Name + " has timed out!");
                    }
                    potentialTrader.Remove(i.Item1);
                }
                else potentialTrader[i.Item1] = i.Item2;
            }
        }

        private void DoTrade()
        {
            string msg = "Trade Successful!";
            string failmsg = "An error occured while trading, possible exploit detected!";
            var thisItems = new List<Item>();
            var targetItems = new List<Item>();

            // make sure trade targets are valid
            if (tradeTarget == null || Owner == null || tradeTarget.Owner == null || Owner != tradeTarget.Owner)
            {
                if (this != null)
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failmsg
                    });

                if (tradeTarget != null)
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failmsg
                    });
                var dir = @"logs"; //start of logging code
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\DupeLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + nName + " has attempted to dupe with " + tradeTarget.nName);
                } //end of logging code
                //A good idea would to log this to a .txt or have an auto-ban checker such as my anticheat.
                //Just looked at it in-game... it spams you with the failmsg. Working on fix
                return;
            }

            //get trade items
            for (int i = 4; i < Inventory.Length; i++)
            {
                if (trade[i] && !Inventory[i].Soulbound)
                {
                    thisItems.Add(Inventory[i]);
                    Inventory[i] = null;
                    UpdateCount++;

                    //save this trade info
                    if (itemnumber1 == 0)
                    {
                        items1 = items1 + " " + thisItems[itemnumber1].ObjectId;
                    }
                    else if (itemnumber1 > 0)
                    {
                        items1 = items1 + ", " + thisItems[itemnumber1].ObjectId;
                    }
                    itemnumber1++;
                }


                if (tradeTarget.trade[i] && !tradeTarget.Inventory[i].Soulbound)
                {
                    targetItems.Add(tradeTarget.Inventory[i]);
                    tradeTarget.Inventory[i] = null;
                    tradeTarget.UpdateCount++;

                    // save target trade info
                    if (itemnumber2 == 0)
                    {
                        items2 = items2 + " " + targetItems[itemnumber2].ObjectId;
                    }
                    else if (itemnumber2 > 0)
                    {
                        items2 = items2 + ", " + targetItems[itemnumber2].ObjectId;
                    }
                    itemnumber2++;
                }
            }

            // move thisItems -> tradeTarget
            for (int j = thisItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < tradeTarget.Inventory.Length; i++)
                {
                    if ((tradeTarget.SlotTypes[i] == 0 &&
                    tradeTarget.Inventory[i] == null) ||
                    (thisItems[j] != null &&
                    tradeTarget.SlotTypes[i] == thisItems[j].SlotType &&
                    tradeTarget.Inventory[i] == null))
                    {
                        tradeTarget.Inventory[i] = thisItems[j];
                        thisItems.RemoveAt(j);
                        break;
                    }
                }

            //move tradeItems -> this
            for (int j = targetItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if ((SlotTypes[i] == 0 &&
                    Inventory[i] == null) ||
                    (targetItems[j] != null &&
                    SlotTypes[i] == targetItems[j].SlotType &&
                    Inventory[i] == null))
                    {
                        Inventory[i] = targetItems[j];
                        targetItems.RemoveAt(j);
                        break;
                    }
                }

            //check for lingering items
            if (thisItems.Count > 0 ||
            targetItems.Count > 0)
            {
                msg = "An error occured while trading!";
            }

            // trade successful, notify and save
            psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            tradeTarget.psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            SaveToCharacter();
            psr.Save();
            tradeTarget.SaveToCharacter();
            tradeTarget.psr.Save();


            //clean
            items1 = "";
            items2 = "";
            itemnumber1 = 0;
            itemnumber2 = 0;
            UpdateCount++;
            tradeTarget.UpdateCount++;
            tradeTarget.tradeTarget = null;
            tradeTarget.trade = null;
            tradeTarget.tradeAccepted = false;
            tradeTarget = null;
            trade = null;
            tradeAccepted = false;
        }
    }
}
//This code should be pasted on and after "public void DoTrade" (This is older code that doesn't have the duping fix, kept it just in case...)
/*
 private void DoTrade()
{
string msg = "Trade Successful!";
string failmsg = "An error occured while trading!";
var thisItems = new List<Item>();
var targetItems = new List<Item>();

// make sure trade targets are valid
if (tradeTarget == null || Owner == null || tradeTarget.Owner == null || Owner != tradeTarget.Owner)
{
if (this != null)
psr.SendPacket(new TradeDonePacket
{
Result = 1,
Message = failmsg
});

if (tradeTarget != null)
tradeTarget.psr.SendPacket(new TradeDonePacket
{
Result = 1,
Message = failmsg
});

//TODO - logThis
return;
}

// get trade items
for (int i = 4; i < Inventory.Length; i++)
{
if (trade[i] && !Inventory[i].Soulbound)
{
thisItems.Add(Inventory[i]);
Inventory[i] = null;
UpdateCount++;

// save this trade info
if (itemnumber1 == 0)
{
items1 = items1 + " " + thisItems[itemnumber1].ObjectId;
}
else if (itemnumber1 > 0)
{
items1 = items1 + ", " + thisItems[itemnumber1].ObjectId;
}
itemnumber1++;
}


if (tradeTarget.trade[i] && !tradeTarget.Inventory[i].Soulbound)
{
targetItems.Add(tradeTarget.Inventory[i]);
tradeTarget.Inventory[i] = null;
tradeTarget.UpdateCount++;

// save target trade info
if (itemnumber2 == 0)
{
items2 = items2 + " " + targetItems[itemnumber2].ObjectId;
}
else if (itemnumber2 > 0)
{
items2 = items2 + ", " + targetItems[itemnumber2].ObjectId;
}
itemnumber2++;
}
}

// move thisItems -> tradeTarget
for (int j = thisItems.Count - 1; j >= 0; j--)
for (int i = 0; i < tradeTarget.Inventory.Length; i++)
{
if ((tradeTarget.SlotTypes[i] == 0 &&
tradeTarget.Inventory[i] == null) ||
(thisItems[j] != null &&
tradeTarget.SlotTypes[i] == thisItems[j].SlotType &&
tradeTarget.Inventory[i] == null))
{
tradeTarget.Inventory[i] = thisItems[j];
thisItems.RemoveAt(j);
break;
}
}

// move tradeItems -> this
for (int j = targetItems.Count - 1; j >= 0; j--)
for (int i = 0; i < Inventory.Length; i++)
{
if ((SlotTypes[i] == 0 &&
Inventory[i] == null) ||
(targetItems[j] != null &&
SlotTypes[i] == targetItems[j].SlotType &&
Inventory[i] == null))
{
Inventory[i] = targetItems[j];
targetItems.RemoveAt(j);
break;
}
}

// check for lingering items
if (thisItems.Count > 0 ||
targetItems.Count > 0)
{
msg = "An error occured while trading!";
}

// trade successful, notify and save
psr.SendPacket(new TradeDonePacket
{
Result = 1,
Message = msg
});
tradeTarget.psr.SendPacket(new TradeDonePacket
{
Result = 1,
Message = msg
});
SaveToCharacter();
psr.Save();
tradeTarget.SaveToCharacter();
tradeTarget.psr.Save();

// clean up
items1 = "";
items2 = "";
itemnumber1 = 0;
itemnumber2 = 0;
UpdateCount++;
tradeTarget.UpdateCount++;
tradeTarget.tradeTarget = null;
tradeTarget.trade = null;
tradeTarget.tradeAccepted = false;
tradeTarget = null;
trade = null;
tradeAccepted = false;
}
}
}
 */


//Here is the fix for the dupe and item slot glitch
/*
private void DoTrade()
        {
            string msg = "Trade Successful!";
            string failmsg = "An error occured while trading, possible exploit detected!";
            var thisItems = new List<Item>();
            var targetItems = new List<Item>();

            // make sure trade targets are valid
            if (tradeTarget == null || Owner == null || tradeTarget.Owner == null || Owner != tradeTarget.Owner)
            {
                if (this != null)
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failmsg
                    });

                if (tradeTarget != null)
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failmsg
                    });
                var dir = @"logs"; //start of logging code
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\DupeLog.txt", true))
                {
                    writer.WriteLine("[" + DateTime.Now + "]" + nName + " has attempted to dupe with " + tradeTarget.nName);
                } //end of logging code
                //A good idea would to log this to a .txt or have an auto-ban checker such as my anticheat.
                //Just looked at it in-game... it spams you with the failmsg. Working on fix
                return;
            }

            //get trade items
            for (int i = 4; i < Inventory.Length; i++)
            {
                if (trade[i] && !Inventory[i].Soulbound)
                {
                    thisItems.Add(Inventory[i]);
                    Inventory[i] = null;
                    UpdateCount++;

                    //save this trade info
                    if (itemnumber1 == 0)
                    {
                        items1 = items1 + " " + thisItems[itemnumber1].ObjectId;
                    }
                    else if (itemnumber1 > 0)
                    {
                        items1 = items1 + ", " + thisItems[itemnumber1].ObjectId;
                    }
                    itemnumber1++;
                }


                if (tradeTarget.trade[i] && !tradeTarget.Inventory[i].Soulbound)
                {
                    targetItems.Add(tradeTarget.Inventory[i]);
                    tradeTarget.Inventory[i] = null;
                    tradeTarget.UpdateCount++;

                    // save target trade info
                    if (itemnumber2 == 0)
                    {
                        items2 = items2 + " " + targetItems[itemnumber2].ObjectId;
                    }
                    else if (itemnumber2 > 0)
                    {
                        items2 = items2 + ", " + targetItems[itemnumber2].ObjectId;
                    }
                    itemnumber2++;
                }
            }

            // move thisItems -> tradeTarget
            for (int j = thisItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < tradeTarget.Inventory.Length; i++)
                {
                    if ((tradeTarget.SlotTypes[i] == 0 &&
                    tradeTarget.Inventory[i] == null) ||
                    (thisItems[j] != null &&
                    tradeTarget.SlotTypes[i] == thisItems[j].SlotType &&
                    tradeTarget.Inventory[i] == null))
                    {
                        tradeTarget.Inventory[i] = thisItems[j];
                        thisItems.RemoveAt(j);
                        break;
                    }
                }

            //move tradeItems -> this
            for (int j = targetItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if ((SlotTypes[i] == 0 &&
                    Inventory[i] == null) ||
                    (targetItems[j] != null &&
                    SlotTypes[i] == targetItems[j].SlotType &&
                    Inventory[i] == null))
                    {
                        Inventory[i] = targetItems[j];
                        targetItems.RemoveAt(j);
                        break;
                    }
                }

            //check for lingering items
            if (thisItems.Count > 0 ||
            targetItems.Count > 0)
            {
                msg = "An error occured while trading!";
            }

            // trade successful, notify and save
            psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            tradeTarget.psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            SaveToCharacter();
            psr.Save();
            tradeTarget.SaveToCharacter();
            tradeTarget.psr.Save();
            

            //clean
            items1 = "";
            items2 = "";
            itemnumber1 = 0;
            itemnumber2 = 0;
            UpdateCount++;
            tradeTarget.UpdateCount++;
            tradeTarget.tradeTarget = null;
            tradeTarget.trade = null;
            tradeTarget.tradeAccepted = false;
            tradeTarget = null;
            trade = null;
            tradeAccepted = false;
        }
*/