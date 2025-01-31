﻿using SoloClient;
using System.Net.Sockets;
using System.Text;

namespace SocketLibTester.SocketHelpers
{
    class State
    {
        public Socket StateSocket { get; set; } = null;
        public int BufferMaxSize { get; private set; }
        public byte[] Buffer { get; set; }
        public StringBuilder StringBuffer { get; set; } = new StringBuilder();
        public Client StateClient { get; private set; } = null;
        public ClientGUI StateForm { get; set; }

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

        public State(int buffMaxSize, Client client)
        {
            BufferMaxSize = buffMaxSize;
            Buffer = new byte[BufferMaxSize];
            StateClient = client;
        }
    }
}
