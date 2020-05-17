using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLibTester.SocketHelpers
{
    class AsyncServer
    {
        public string Ip { get; private set; } = "127.0.0.1";
        public int Port { get; private set; } = 8080;
        public Command[] Commands { get; private set; }

        public static ManualResetEvent AllDone { get; private set; } = new ManualResetEvent(false);

        private Thread _thread;

        public AsyncServer()
        {
        }

        public AsyncServer(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public AsyncServer(Command[] cmds)
        {
            Commands = cmds;
        }

        public void Start()
        {
            _thread = new Thread(() =>
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    while (true)
                    {
                        AllDone.Reset();

                        Console.WriteLine("Waiting for a connection...");
                        State state = new State(1024, this);
                        state.StateSocket = listener;
                        listener.BeginAccept(
                            new AsyncCallback(Helper.AcceptCallback),
                            state);

                        AllDone.WaitOne();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Abort();
        }
    }
}
