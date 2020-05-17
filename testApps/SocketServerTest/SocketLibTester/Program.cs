using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketLibTester.SocketHelpers;

namespace SocketLibTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Command hello = new Command("hello", (state, parts) =>
            {
                return $"hello variant {parts[1]}";
            });
            Command bye = new Command("bye", (state, parts) =>
            {
                return $"bye variant {parts[1]}";
            });
            AsyncServer server = new AsyncServer(new Command[] { hello, bye } );
            server.Start();
            //Thread thread = new Thread(() =>
            //{
            //    Client client = new Client();
            //    client.Start();
            //    int i = 0;
            //    while (true)
            //    {
            //        Console.WriteLine(client.SendMessage($"hello {i}"));
            //        ++i;
            //        Thread.Sleep(1000);
            //    }

            //});
            //thread.Start();
        }
    }
}
