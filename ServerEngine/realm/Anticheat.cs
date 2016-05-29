using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ServerEngine.realm;
using System.Net.NetworkInformation;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;
using System.Reflection;
using System.Globalization;
using ServerEngine.realm.commands;

namespace ServerEngine.Anticheat
{
	//public bool UseMpCost(RealmItem item)
	//{
	//	if (Mp - item.MpCost < 0 || HasConditionEffect(ConditionEffectBit.Quiet) || HasConditionEffect(ConditionEffectBit.Paused))
	//		return false;

	//	Mp -= item.MpCost;
	//	UpdateCount++;
	//	return true;
	//}


	//public bool UseItem(UseItem msg, RealmItem item)
	//{
	//	return UseMpCost(item) && item.ActivateEffects.Aggregate(false, (current, effect) => (useItemEffect(effect, msg, item) && item.Consumable) || current);
	//}
		//else if (AudioEngine.Time - audioCheckTime >= 1000)
	 //               {
	 //                   float rate = 1;
	 //                   if (ModManager.CheckActive(Mods.DoubleTime))
	 //                       rate = 1.5f;
	 //                   else if (ModManager.CheckActive(Mods.HalfTime))
	 //                       rate = 0.75f;
	 
	 //                   float diff = (AudioEngine.Time - audioCheckTime) * (1f / rate);
	 
	 //                   if (Math.Abs(diff - (GameBase.Time - audioCheckTimeComp)) > 60)
	 //                   {
	 //                       if (++audioCheckCount > 5)
	 //                       {
	 //                           NotificationManager.ShowMessage(
	 //                               "There was an error during timing calculations.  If you continue to get this error, please update your AUDIO/SOUND CARD drivers!  Your score will not be submitted for this play.");
	 //                           //BanchoClient.HandleException(new Exception("timing error"), "user is hacking?");
	 //                           currentScore.InvalidateSubmission();
	 //                           GameBase.Scheduler.AddDelayed(delegate { flag |= BadFlags.SpeedHackDetected; }, 200);
	 //                           audioCheckCount = -500;
	 //                       }
	 //                   }
	 //                   else
	 //                       audioCheckCount = 0;
	 //                   audioCheckTime = -1;
	 //               }




}
