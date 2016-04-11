using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace ConServer
{
    class Program
    {
        static HttpListener listener;
        static Thread listen;
        static readonly Thread[] workers = new Thread[5];
        static readonly Queue<HttpListenerContext> contextQueue = new Queue<HttpListenerContext>();
        static readonly object queueLock = new object();
        static readonly ManualResetEvent queueReady = new ManualResetEvent(false);

        public XmlDatas GameData { get; private set; }

        const int port = 8888; //Can use 80, 8080, 8888, or possibly 443 (make sure to change in client)

        static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + port + "/"); //add IP of your website to link to gold purchase OR leave blank and create your own hard-coded website!
            listener.Start(); //Server will freak out if you don't have admin permissions

            listen = new Thread(ListenerCallback);
            listen.Start();
            for (var i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(Worker);
                workers[i].Start();
            }
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Terminating...");
                listener.Stop();
                while (contextQueue.Count > 0)
                    Thread.Sleep(100);
                Environment.Exit(0);
            };
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "Database Server";
            Console.WriteLine("Connection Successful at Port " + port + ".");
            XmlDatas.behaviors = false;
            XmlDatas.DoSomething();
            Thread.CurrentThread.Join();
        }

        static void ListenerCallback()
        {
            try
            {
                do
                {
                    var context = listener.GetContext();
                    lock (queueLock)
                    {
                        contextQueue.Enqueue(context);
                        queueReady.Set();
                    }
                } while (true);
            }
            catch
            {

            }
        }

        static void Worker()
        {
            while (queueReady.WaitOne())
            {
                HttpListenerContext context;
                lock (queueLock)
                {
                    if (contextQueue.Count > 0)
                        context = contextQueue.Dequeue();
                    else
                    {
                        queueReady.Reset();
                        continue;
                    }
                }

                try
                {
                    ProcessRequest(context);
                }
                catch
                {

                }
            }
        }

        static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                IRequestHandler handler;

                if (!RequestHandlers.Handlers.TryGetValue(context.Request.Url.LocalPath, out handler))
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "Error 404. Page Not Found.";
                    using (StreamWriter wtr = new StreamWriter(context.Response.OutputStream))
                        wtr.Write("<h1>Error 404. Page Not Found.</h1>");
                }
                else
                    handler.HandleRequest(context);
            }
            catch
            {
                using (StreamWriter wtr = new StreamWriter(context.Response.OutputStream))
                    wtr.Write("<Error>Global server error. Please write new error.</Error>");
                    //This error SHOULD return for all server errors that do NOT have their own error catch, otherwise, this should not be displayed.
            }
            context.Response.Close();
        }
    }
}
