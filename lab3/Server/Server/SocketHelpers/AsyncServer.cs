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
    class AsyncServer
    {
        public string Ip { get; private set; } = "127.0.0.1";
        public int Port { get; private set; } = 8080;
        public Command[] Commands { get; private set; }

        public static ManualResetEvent AllDone { get; private set; } = new ManualResetEvent(false);
        public delegate void AddLogDelegate(string msg);
        public AddLogDelegate addLog;

        private Thread _thread;
        Socket _listener;
        private State _currState;

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

        public AsyncServer(string ip, int port, AddLogDelegate addLog, Command[] cmds)
        {
            Ip = ip;
            Port = port;
            Commands = cmds;
            this.addLog = addLog;
        }

        public void Start(CancellationToken ct)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            addLog("Сервер готов к подключениям");

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                while (true)
                {
                    AllDone.Reset();

                    State state = new State(1024, this);
                    state.StateSocket = _listener;
                    _currState = state;
                    _listener.BeginAccept(
                        new AsyncCallback(Helper.AcceptCallback),
                        state);
                    AllDone.WaitOne();
                    if (ct.IsCancellationRequested)
                    {
                        state.StateServer.Ip = "";
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                addLog(e.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                _listener.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e) { }
            finally
            {
                _listener.Close();
            }
            //_listener.Shutdown(SocketShutdown.Both);
            //if (_currState == null)
            //    _listener.Close();
            //else
            //    _currState.StateSocket.Close();
            addLog("Подключение деактивировано");
        }
    }
}
