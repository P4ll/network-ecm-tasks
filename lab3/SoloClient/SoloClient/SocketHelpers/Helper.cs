using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

                Task.Factory.StartNew(() =>
                {
                    state.StateClient.AddMainLog($"Клиент подключен с адресом: {state.StateSocket.LocalEndPoint}");
                    state.StateForm.SafeChangeConnection();
                });
                Client.ConnectDone.Set();
            }
            catch (Exception e)
            {
                Task.Factory.StartNew(() =>
                {
                    state.StateClient.AddMainLog("Ошибка соединения с сервером");
                });
                Client.ConnectDone.Set();
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
                        string recMsg = state.StringBuffer.ToString();
                        recMsg = recMsg.Trim('\n');
                        recMsg = recMsg.Trim('\r');
                        state.StringBuffer.Clear();
                        Task.Factory.StartNew(() =>
                        {
                            state.StateClient.AddLog(recMsg);
                        });
                        Client.ReceiveDone.Set();
                    }

                    client.BeginReceive(state.Buffer, 0, state.BufferMaxSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    Client.ReceiveDone.Set();
                    state.StateClient.AddLog(state.StringBuffer.ToString());
                }
            }
            catch (Exception e)
            {
            }
        }

    }
}
