using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using log4net;

namespace ServerEngine.realm
{
	public class LogicTicker //Sync our server with CPU or using our own formulas
	{
		static ILog log = LogManager.GetLogger(typeof(LogicTicker));
		public const int TPS = 20; //Ticks Per Second                https://www.youtube.com/watch?v=DoexBWfjWUg
		public const int MsPT = 1000 / TPS; //Milliseconds Per Tick (divided by our tps)
		// 50 ms of lag is generally the most accepted ratio, as we see here we divide 1000 by 20 to get 50, our server to client latency
		public LogicTicker()
		{
			pendings = new ConcurrentQueue<Action<RealmTime>>[5];
			for (int i = 0; i < 5; i++)
				pendings[i] = new ConcurrentQueue<Action<RealmTime>>();
		}

		public void AddPendingAction(Action<RealmTime> callback)
		{
			AddPendingAction(callback, PendingPriority.Normal);
		}
		public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
		{
			pendings[(int)priority].Enqueue(callback);
		}
		readonly ConcurrentQueue<Action<RealmTime>>[] pendings;
		public static RealmTime CurrentTime;
		public void TickLoop() //Loop our game world
		{
			Stopwatch watch = new Stopwatch();
			long dt = 0; //Delta Time
			long count = 0; //Counts
			watch.Start();
			RealmTime t = new RealmTime(); // t is our RealmTime
			long xa = 0; //random creation
			do
			{
				long times = dt / MsPT; //get our times, divide delta time by our 1000 MsPT, get 0
				//Operators, -= is decrementing and += is incrementing
				dt -= times * MsPT; //Here, dt is decremented from times * our 1000 (times = 0) Here, dt is still zero so we don't actually decrement
				times++; //Add 1 to times
				long b = watch.ElapsedMilliseconds; //b = time lapsed from our stopwatch starting
				count += times; //Increment count into times
				if (times > 3) //Once times is greater than 3 (times = refer to above), print to console
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("Pinged! times pinged:" + times + " dt:" + dt + " count:" + count + " time:" + b + " tps:" + count / (b / 1000.0));
					Console.ForegroundColor = ConsoleColor.White;
				}
				t.tickTimes = b;
				t.tickCount = count;
				t.thisTickCounts = (int)times; 
				t.thisTickTimes = (int)(times * MsPT);
				xa += t.thisTickTimes; //Increment xa into TickTimes (times * MsPT)
				foreach (var i in pendings)
				{
					Action<RealmTime> callback;
					while (i.TryDequeue(out callback))
					{
						try
						{
							callback(t);
						}
						catch (Exception ex)
						{
							Console.ForegroundColor = ConsoleColor.Yellow;
							Console.Out.WriteLine("Logic or tick error, check LogicTicker.cs");
							log.Error(ex);
							Console.ForegroundColor = ConsoleColor.White;
						}
					}
				}
				TickWorlds1(t);
				Thread.Sleep(MsPT); //Let the server sleep by 1000 ms, not letting the thread sleep will cause everything to go super saiyan
				dt += Math.Max(0, 0 - MsPT); //Refer below
											 //dt += Math.Max(0, watch.ElapsedMilliseconds - b - MsPT);
											 /*
											 Basically our delta time is incremented into our equation. dt +(1) <- 0, b - b - MsPT
											 why b - b I have no idea
											 */
				//GC.Collect(); //Lags everything, not worth implementing unless done right
			} while (true);
		}
		void TickWorlds1(RealmTime t)    //Continous simulation, keeps our game world running
		{
			CurrentTime = t;
			foreach (var i in RealmManager.Worlds.Values.Distinct())
				i.Tick(t);
		}
		/*
		Our discrete version isn't necessary to run the server, it is only necessary for debugging really
		We could also use it for daytime events, example being the alchemist
		*/
		//void TickWorlds2(RealmTime t)    //Discrete simulation, used to count the ticks, can be printed to console.
		//{
		//    long counter = t.thisTickTimes;
		//    long c = t.tickCount - t.thisTickCounts;
		//    long x = t.tickTimes - t.thisTickTimes;
		//    while (counter >= MsPT)
		//    {
		//        c++; x += MsPT;
		//        TickWorlds1(new RealmTime()
		//        {
		//            thisTickCounts = 1,
		//            thisTickTimes = MsPT,
		//            tickCount = c,
		//            tickTimes = x
		//        });
		//        counter -= MsPT;
		//    }
		//}
	}
}
