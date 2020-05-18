using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketLibTester.SocketHelpers
{
    public class Client
    {
        public static ManualResetEvent ConnectDone { get; private set; } = new ManualResetEvent(false);
        public static ManualResetEvent SendDone { get; private set; } = new ManualResetEvent(false);
        public static ManualResetEvent ReceiveDone { get; private set; } = new ManualResetEvent(false);

        public string Ip { get; private set; } = "127.0.0.1";
        public int Port { get; private set; } = 8080;

        public delegate void AddLogDelegate(string msg);
        public AddLogDelegate AddMainLog { get; set; }
        public AddLogDelegate AddLog { get; set; }
        public bool IsConnected { get; set; }

        private IPEndPoint _remoteEP;
        private Socket _client;

        public Client() { }

        public Client(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public void Start(Form form)
        {
            try
            {
                _remoteEP = new IPEndPoint(IPAddress.Parse(Ip), Port);
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                State state = new State(1024, this);
                state.StateSocket = _client;

                _client.BeginConnect(_remoteEP, new AsyncCallback(Helper.ConnectCallback), state);
                ConnectDone.WaitOne();
                form.Show();
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
                State state = new State(1024, this);
                state.StateSocket = _client;

                Helper.SendClient(state, msg);
                SendDone.WaitOne();

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
