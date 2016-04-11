using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using ServerEngine.realm;

namespace ServerEngine //All connection issues can be fixed here.
{
	class NetworkHandler //Main body of packet is written through here
	{
		enum ReceiveState //When server receives packet(s), it does the following
		{
			Awaiting, //waits for packet to queue (let other packets finish)
			ReceivingHdr, //reads the packet header to determine packet ID
			ReceivingBody, //reads the rest of packet
			Processing //server processes the packet
		}
		class ReceiveToken //server checks packet length
		{
			public int Length;
			public Packet Packet;
		}
		enum SendState //Server gets ready to send the packet
		{
			Awaiting, //wait for other packets to finish sending
			Ready, //server is now ready to send packet
			Sending //packet is in transit
		}
		class SendToken //packet is sent (no limiter to data, so any size packet could be sent O.o )
		{
			public Packet Packet;
		}

		public const int BUFFER_SIZE = 0x10000; //buffer
		SocketAsyncEventArgs send; //Packet async
		SendState sendState = SendState.Awaiting;
		ReceiveState receiveState = ReceiveState.Awaiting;
		Socket skt;
		ClientProcessor parent;
		public NetworkHandler(ClientProcessor parent, Socket skt)
		{
			this.parent = parent;
			this.skt = skt;
		}

		public void BeginHandling()
		{
			Console.WriteLine("{0} connected.", skt.RemoteEndPoint);
			//This logging code below is a HUUUUUUUUUGE godsend to anyone hosting on hamachi because anyone playing through hamachi gets a static IP that CANNOT be changed with a proxy or VPN, this makes it one million times easier to track cheating/hacking players on your server. If you're hosting on a VPS however, it's an entirely different story and may as well disable this.
			var dir = @"logs"; //start of logging code
			if (!System.IO.Directory.Exists(dir))
				System.IO.Directory.CreateDirectory(dir);
			//If someone added being able to log usernames in this log and maybe even extra computer information that would be amazing.
			using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\LogOnLog.txt", true)) //HUGE problem I see, if multiple clients connect at once, one or more of the clients won't have their IP logged. Could probably result in ServerEngine crash. NOTE: Just double checked this, multiple clients connecting at once brings up NO problems, but if you're debugging with intellitrace, you will get a popup saying that multiple access's to the file "LogOnLog.txt" was established. Just ignore it and move on. This logging doesn't cause memory problems as far as I can tell.
			{
				writer.WriteLine("[" + DateTime.Now + "]" + skt.RemoteEndPoint + " has connected.");
			} //end of logging code
			skt.NoDelay = true; //don't delay sending thru sockets
			skt.UseOnlyOverlappedIO = true;
			send = new SocketAsyncEventArgs(); //Hmm...
			send.Completed += IOCompleted;
			send.UserToken = new SendToken();
			send.SetBuffer(new byte[BUFFER_SIZE], 0, BUFFER_SIZE);
			var receive = new SocketAsyncEventArgs();
			receive.Completed += IOCompleted;
			receive.UserToken = new ReceiveToken();
			receive.SetBuffer(new byte[BUFFER_SIZE], 0, BUFFER_SIZE);
			receiveState = ReceiveState.ReceivingHdr;
			receive.SetBuffer(0, 5);
			if (!skt.ReceiveAsync(receive)) //if the packet is received...
				IOCompleted(this, receive); //... complete the handle
		}

		void ProcessPolicyFile()
		{
			var s = new NetworkStream(skt);
			NWriter wtr = new NWriter(s);
			wtr.WriteNullTerminatedString(@"<cross-domain-policy>
	        <allowed-access-from domain=""*"" to-ports=""*"" />
            </cross-domain-policy>");
			wtr.Write((byte)'\r');
			wtr.Write((byte)'\n');
			parent.Disconnect();
		}
		
		void IOCompleted(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				bool repeat;
				do
				{
					repeat = false;

					if (e.SocketError != SocketError.Success)
						throw new SocketException((int)e.SocketError);
					if (e.LastOperation == SocketAsyncOperation.Receive)  //possible error
					{
						switch (receiveState)
						{
							case ReceiveState.ReceivingHdr:
								if (e.BytesTransferred < 5) //changing this causes infinite loading
								{
									parent.Disconnect();
									return;
								}

								if (e.Buffer[0] == 0x3c && e.Buffer[1] == 0x70 &&
									e.Buffer[2] == 0x6f && e.Buffer[3] == 0x6c && e.Buffer[4] == 0x69)
								{
									ProcessPolicyFile();
									return;
								} 
								
								var len = (e.UserToken as ReceiveToken).Length =
									IPAddress.NetworkToHostOrder(BitConverter.ToInt32(e.Buffer, 0)) - 5;
								if (len < 0 || len > BUFFER_SIZE)
									throw new InternalBufferOverflowException();
								(e.UserToken as ReceiveToken).Packet = Packet.Packets[(PacketID)e.Buffer[4]].CreateInstance();

								receiveState = ReceiveState.ReceivingBody;
								e.SetBuffer(0, len);
								if (!skt.ReceiveAsync(e))
								{
									repeat = true;
									continue;
								}
								break;
							case ReceiveState.ReceivingBody:
								if (e.BytesTransferred < (e.UserToken as ReceiveToken).Length) //possible fix for random disconnects
								//if the usertoken length is greater than our transferred bytes, disconnect, otherwise, continue
								{
									//parent.Disconnect();
									repeat = true;
									continue;
								}

								var pkt = (e.UserToken as ReceiveToken).Packet;
								pkt.Read(parent, e.Buffer, (e.UserToken as ReceiveToken).Length);

								receiveState = ReceiveState.Processing;
								bool cont = OnPacketReceived(pkt);

								if (cont && skt.Connected)
								{
									receiveState = ReceiveState.ReceivingHdr;
									e.SetBuffer(0, 5);
									if (!skt.ReceiveAsync(e))
									{
										repeat = true;
										continue;
									}
								}
								break;
							default:
								throw new InvalidOperationException(e.LastOperation.ToString());
						}
					}
					else if (e.LastOperation == SocketAsyncOperation.Send)
					{
						switch (sendState)
						{
							case SendState.Ready: //we are ready to send our packet
								var dat = (e.UserToken as SendToken).Packet.Write(parent);
								sendState = SendState.Sending;
								e.SetBuffer(dat, 0, dat.Length);
								if (!skt.SendAsync(e))
								{
									repeat = true;
									continue;
								}
								break;
							case SendState.Sending: //sending our packet
								(e.UserToken as SendToken).Packet = null;
								if (CanSendPacket(e, true))
								{
									repeat = true;
									continue;
								}
								break;
							default:
								throw new InvalidOperationException(e.LastOperation.ToString());
						}
					}
					else
						throw new InvalidOperationException(e.LastOperation.ToString());
				} while (repeat);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error with packet flow, check NetworkHandler.cs");
				Console.ForegroundColor = ConsoleColor.White;
				OnError(ex);
			}
		}


		void OnError(Exception ex)
		{
			parent.Disconnect();
		}
		bool OnPacketReceived(Packet pkt)
		{
			//return parent.ProcessPacket(pkt);
			if (parent.IsReady()) //add the packet to the queue
			{
				RealmManager.Network.AddPendingPacket(parent, pkt);
				return true;
			}
			else
				return false;
		}
		ConcurrentQueue<Packet> pendingPackets = new ConcurrentQueue<Packet>();
		bool CanSendPacket(SocketAsyncEventArgs e, bool ignoreSending) //can we send packets?
		{
			lock (sendLock)
			{
				if (sendState == SendState.Ready ||
					(!ignoreSending && sendState == SendState.Sending))
					return false;
				Packet packet;
				if (pendingPackets.TryDequeue(out packet))
				{
					(e.UserToken as SendToken).Packet = packet;
					sendState = SendState.Ready;
					return true;
				}
				else
				{
					sendState = SendState.Awaiting;
					return false;
				}
			}
		}

		object sendLock = new object();
		public void SendPacket(Packet pkt)
		{
			try
			{
				pendingPackets.Enqueue(pkt);
				if (CanSendPacket(send, false))
				{
					var dat = (send.UserToken as SendToken).Packet.Write(parent);

					sendState = SendState.Sending;
					send.SetBuffer(dat, 0, dat.Length);
					if (!skt.SendAsync(send)) //Causes objects to be thrown
						IOCompleted(this, send);
				}
			}
			catch
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("The following packets were dropped: " + pkt.ID.ToString());
				Console.ForegroundColor = ConsoleColor.White;
				var dir = @"logs"; //start of logging code
				if (!System.IO.Directory.Exists(dir))
					System.IO.Directory.CreateDirectory(dir);
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"logs\DroppedPacketsLog.txt", true)) //logs any dropped packets.
				{
					writer.WriteLine("[" + DateTime.Now + "]" + pkt.ID + " was dropped.");
				} //end of logging code
			}
		}
		public void SendPackets(IEnumerable<Packet> pkts)
		{
			try
			{
				foreach (var i in pkts)
					pendingPackets.Enqueue(i);
				if (CanSendPacket(send, false))
				{
					var dat = (send.UserToken as SendToken).Packet.Write(parent);

					sendState = SendState.Sending;
					send.SetBuffer(dat, 0, dat.Length);
					if (!skt.SendAsync(send))
						IOCompleted(this, send);
				}
			}
			catch
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Some packets were dropped. A problem may have occurred in NetworkHandler.cs");
				Console.ForegroundColor = ConsoleColor.White;
			}
		}
	}
}
/*
Just a word of my advice to anyone that plans on trying to make a huge rotmg pserver: Don't use flash. Flash only supports TCP which 
is a good starting point but not good for fast paced games like realm where lots of data transfer is required. TCP can also hold data or rewind data completely 
which can really screw over anyone playing. This is farther shown since the server uses async to push packets, and if any packets in the queue are pushed before one,
TCP would block it. Overall it's just better to use UDP since the server halfway supports it. The only problem is writing a new client base with your own UDP architecture.

	~Zeroeh
*/