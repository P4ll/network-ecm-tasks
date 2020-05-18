using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace SocketLibTester.SocketHelpers
{
    class State
    {
        public Socket StateSocket { get; set; } = null;
        public int BufferMaxSize { get; private set; }
        public byte[] Buffer { get; set; }
        public StringBuilder StringBuffer { get; set; } = new StringBuilder();
        public AsyncServer StateServer { get; private set; } = null;
        public Command LastCommad { get; set; }
        
        public State()
        {
            BufferMaxSize = 1024;
            Buffer = new byte[BufferMaxSize];
        }

        public State(int buffMaxSize)
        {
            BufferMaxSize = buffMaxSize;
            Buffer = new byte[BufferMaxSize];
        }

        public State(int buffMaxSize, AsyncServer server)
        {
            BufferMaxSize = buffMaxSize;
            Buffer = new byte[BufferMaxSize];
            StateServer = server;
        }
    }
}
