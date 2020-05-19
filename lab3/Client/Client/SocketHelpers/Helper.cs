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
        public static void SendClient(State state, String data)
        {
            state.StringBuffer.Clear();
            state.StringBuffer.Append(data);
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            state.StateSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallbackClient), state);
        }

        public static void SendCallbackClient(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            try
            {
                Socket client = state.StateSocket;

                int bytesSent = client.EndSend(ar);

                Client.SendDone.Set();
            }
            catch (Exception e)
            {
                state.StateClient.AddLog("Не удалось отправить на сервер");
            }
        }

        public static void ConnectCallback(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            try
            {
                Socket client = state.StateSocket;

                client.EndConnect(ar);

                state.StateClient.AddMainLog($"Клиент подключен к {client.LocalEndPoint}");

                Client.ConnectDone.Set();
            }
            catch (Exception e)
            {
                state.StateClient.AddMainLog("Ошибка соединения с сервером");
                if (state.StateForm != null && state.StateForm.IsAccessible)
                    state.StateForm.SafeClose();
            }
        }

        public static void Receive(ref State state)
        {
            try
            {
                state.StringBuffer.Clear();

                state.StateSocket.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                state.StateClient.AddLog("Не удалось получить данные с сервера");
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            try
            {
                Socket client = state.StateSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.StringBuffer.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    if (state.StringBuffer.ToString().IndexOf("\r\n") > -1)
                    {
                        Task task = Task.Factory.StartNew(() =>
                        {
                            string recMsg = state.StringBuffer.ToString();
                            recMsg = recMsg.Trim('\n');
                            recMsg = recMsg.Trim('\r');
                            state.StringBuffer.Clear();
                            state.StateClient.AddLog(recMsg);
                        });
                        Client.ReceiveDone.Set();
                    }

                    client.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    //if (state.StringBuffer.Length > 1)
                    //{
                    //}
                    Client.ReceiveDone.Set();
                    state.StateClient.AddLog(state.StringBuffer.ToString());
                }
            }
            catch (Exception e)
            {
                state.StateClient.AddLog("Не удалось получить данные с сервера");
            }
        }

    }
}
