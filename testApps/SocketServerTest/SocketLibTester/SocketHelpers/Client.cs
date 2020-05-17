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

    }
}
