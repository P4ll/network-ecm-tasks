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
            state.StateServer.addLog("Соединение с клиентом установлено");
            handler.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            State state = (State)ar.AsyncState;
            if (state.StateServer.Ip == "")
            {
                state.StateSocket.Shutdown(SocketShutdown.Both);
                state.StateSocket.Close();
                return;
            }

            try
            {
                Socket handler = state.StateSocket;

                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.StringBuffer.Append(Encoding.ASCII.GetString(
                        state.Buffer, 0, bytesRead));

                    content = state.StringBuffer.ToString();

                    if (content.IndexOf("\r\n") > -1)
                    {
                        content = content.Trim('\n');
                        content = content.Trim('\r');
                        state.StateServer.addLog(String.Format("Пришло {0} байт. \n Данные: {1}",
                            content.Length, content));
                        string[] cmdParts = content.Split(' ');
                        cmdParts[0].ToLower();
                        bool isCommand = false;
                        for (int i = 0; i < state.StateServer.Commands.Length; ++i)
                        {
                            if (cmdParts[0] == state.StateServer.Commands[i].Cmd)
                            {
                                content = state.StateServer.Commands[i].GetRespose(state, cmdParts);
                                state.LastCommad = state.StateServer.Commands[i];
                                isCommand = true;
                                break;
                            }
                        }
                        if (!isCommand)
                        {
                            content = "Unexpected command, type help";
                        }
                        Send(state, content);

                    }
                    else
                    {
                        handler.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }

            }
            catch (Exception e)
            {
                state.StateServer.addLog("Не удалось прочитать сообщение");
            }
        }

        public static void Send(State state, String data)
        {

            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            Socket handler = state.StateSocket;

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                State state = (State)ar.AsyncState;
                Socket handler = state.StateSocket;

                int bytesSent = handler.EndSend(ar);

                state.StateServer.addLog($"Сообщение({bytesSent} байт) отправлено клиенту");
                state.StringBuffer.Clear();
                if (state.LastCommad != null && state.LastCommad.IsCloseCommand)
                {
                    handler.Shutdown(SocketShutdown.Receive);
                    handler.Close();
                    return;
                }
                handler.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
