using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketLibTester.SocketHelpers
{
    class Command
    {
        public string Cmd { get; set; }
        public delegate string Handler(State state, string[] parts);
        private Handler _handler;

        public Command(string cmd, Handler handler)
        {
            Cmd = cmd;
            _handler = handler;
        }

        public string GetRespose(State state, string[] parts)
        {
            return _handler(state, parts);
        }
    }
}
