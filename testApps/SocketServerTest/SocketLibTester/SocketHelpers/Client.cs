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
    class Client
    {
        public static ManualResetEvent ConnectDone { get; private set; } = new ManualResetEvent(false);
        public static ManualResetEvent SendDone { get; private set; } = new ManualResetEvent(false);
        public static ManualResetEvent ReceiveDone { get; private set; } = new ManualResetEvent(false);

        public string Ip { get; private set; } = "127.0.0.1";
        public int Port { get; private set; } = 8080;

        private IPEndPoint _remoteEP;
        private Socket _client;

        public Client() { }

        public Client(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public void Start()
        {
            try
            {
                _remoteEP = new IPEndPoint(IPAddress.Parse(Ip), Port);
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _client.BeginConnect(_remoteEP, new AsyncCallback(Helper.ConnectCallback), _client);
                ConnectDone.WaitOne();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string SendMessage(string msg)
        {
            try
            {
                Helper.SendClient(_client, $"{msg}<EOF>");
                SendDone.WaitOne();

                State state = new State(1024, this);
                state.StateSocket = _client;

                Helper.Receive(ref state);
                ReceiveDone.WaitOne();

                return state.StringBuffer.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }

        public void Stop()
        {
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
    }
}
