using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ServerEngine.realm;
using System.Net.NetworkInformation;
using ServerEngine.cliPackets;
using ServerEngine.svrPackets;
using System.Reflection;
using System.Globalization;
using log4net;

namespace ServerEngine
{
	static class Program
	{
		static Socket svrSkt;

		const int port = 2050;

		static void HostPolicyServer()
		{
			try
			{
				TcpListener listener = new TcpListener(IPAddress.Any, 843); //can't change 843 without client edits?
				listener.Start();
				listener.BeginAcceptTcpClient(ServePolicyFile, listener);
			}
			catch
			{

			}
		}

		static void Main(string[] args) //Main
		{
			ILog log = LogManager.GetLogger(typeof(Program)); //Added
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			svrSkt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			svrSkt.Bind(new IPEndPoint(IPAddress.Any, 2050));
			svrSkt.Listen(0xff);
			svrSkt.BeginAccept(Listen, null);
			Console.CancelKeyPress += (sender, e) =>
			{
				Console.WriteLine("Saving Please Wait...");
				svrSkt.Close();
				foreach (var i in RealmManager.Clients.Values.ToArray())
				{
					i.Save();
					i.Disconnect();
				}
				Console.WriteLine("\nClosing...");
				Thread.Sleep(100);
				Environment.Exit(0);
			};

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Title = "Server Engine";
			Console.WriteLine("Accepting connections at port " + port + ".");
			HostPolicyServer();
			RealmManager.CoreTickLoop();
		}

		static void ServePolicyFile(IAsyncResult ar)
		{
			TcpClient cli = (ar.AsyncState as TcpListener).EndAcceptTcpClient(ar);
			(ar.AsyncState as TcpListener).BeginAcceptTcpClient(ServePolicyFile, ar.AsyncState);
			try
			{
				var s = cli.GetStream();
				NReader rdr = new NReader(s);
				NWriter wtr = new NWriter(s);
				if (rdr.ReadNullTerminatedString() == "<policy-file-request/>")
				{
					wtr.WriteNullTerminatedString(@"<cross-domain-policy>
	                <allow-access-from domain=""*"" to-ports=""*"" />
                    </cross-domain-policy>");
					wtr.Write((byte)'\r');
					wtr.Write((byte)'\n');
				}
				cli.Close();
			}
			catch
			{

			}
		}

		static void Listen(IAsyncResult ar)
		{
			Socket skt = null;
			try
			{
				skt = svrSkt.EndAccept(ar);
			}
			catch (ObjectDisposedException)
			{ 
			}
			try
			{
				svrSkt.BeginAccept(Listen, null);
			}
			catch (ObjectDisposedException)
			{
			}
			if (skt != null)
			{
				var psr = new ClientProcessor(skt);
				psr.BeginProcess();
			}
		}
	}
}
