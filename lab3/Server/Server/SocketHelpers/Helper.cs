using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SocketLibTester.SocketHelpers
{
    static class Helper
    {
        public static void AcceptCallback(IAsyncResult ar)
        {
            AsyncServer.AllDone.Set();
            State state = (State)ar.AsyncState;
            Socket listener = state.StateSocket;
            if (state.StateServer.Ip == "")
                return;
            Socket handler = listener.EndAccept(ar);

            state.StateSocket = handler;
            handler.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            State state = (State)ar.AsyncState;
            Socket handler = state.StateSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.StringBuffer.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));

                content = state.StringBuffer.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    state.StateServer.addLog(String.Format("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content));
                    string[] cmdParts = content.Split(' ');
                    cmdParts[0].ToLower();

                    for (int i = 0; i < state.StateServer.Commands.Length; ++i)
                    {
                        if (cmdParts[0] == state.StateServer.Commands[i].Cmd)
                        {
                            content = state.StateServer.Commands[i].GetRespose(state, cmdParts);
                        }
                    }
                    Send(handler, content);

                }
                else
                {
                    handler.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        public static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SendClient(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallbackClient), handler);
        }

        public static void SendCallbackClient(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                Client.SendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                Client.ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Receive(ref State state)
        {
            try
            {
                state.StateSocket.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                State state = (State)ar.AsyncState;
                Socket client = state.StateSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.StringBuffer.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    client.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.StringBuffer.Length > 1)
                    {
                        Console.WriteLine($"Response: {state.StringBuffer.ToString()}");
                    }
                    Client.ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
