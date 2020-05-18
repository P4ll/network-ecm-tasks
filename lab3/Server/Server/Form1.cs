using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketLibTester.SocketHelpers;

namespace Server
{
    public partial class Form1 : Form
    {
        private AsyncServer _server;
        private bool _connectionState = false;
        private delegate void SafeCallDelegate(string msg);
        private Task _task;
        private CancellationTokenSource _token;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnMainAct_Click(object sender, EventArgs e)
        {
            if (_connectionState == false)
            {
                string ip = ipTextBox.Text;
                string portStr = portTextBox.Text;

                ip = ipCheck(ip);
                int port = portCheck(portStr);

                if (ip == "" || port == -1)
                    return;

                _server = new AsyncServer(ip, port, AddLog, new Command[] {
                    new Command("hello", (state, parts) =>
                    {
                        return $"hello variant {parts[1]}";
                    }),
                    new Command("bye", true, (state, parts) =>
                    {
                        if (parts.Length == 1)
                            return "bye";
                        return $"bye variant {parts[1]}";
                    }),
                });

                _token = new CancellationTokenSource();
                CancellationToken ct = _token.Token;

                _task = new Task(() => {
                    _server.Start(ct);
                }, ct);
                _task.Start();

                btnMainAct.Text = "Деактивировать";
                _connectionState = true;
            }
            else
            {
                AsyncServer.AllDone.Set();
                _token.Cancel();
                Task.Factory.StartNew(() =>
                {
                    _server.Stop();
                });
                btnMainAct.Text = "Активировать";
                _connectionState = false;
            }
        }

        private string ipCheck(string ip)
        {
            for (int i = 0; i < ip.Length; ++i)
            {
                if (!Char.IsDigit(ip[i]) && ip[i] != '.')
                {
                    AddLog("Ошибка в адресе");
                    return "";
                }
            }

            string[] parts = ip.Split('.');
            foreach (string part in parts)
            {
                int c = -1;
                if (!Int32.TryParse(part, out c) || c < 0 || c > 255)
                {
                    AddLog("Ошибка в адресе");
                    return "";
                }
            }
            return ip;
        }

        private int portCheck(string port)
        {
            int portInt = -1;
            if (!Int32.TryParse(port, out portInt))
            {
                AddLog("Ошибка в значении порта");
                return -1;
            }
            return portInt;
        }

        public void AddLog(string msg)
        {
            if (consoleTextBox.InvokeRequired)
            {
                SafeCallDelegate d = new SafeCallDelegate(AddLog);
                consoleTextBox.Invoke(d, new object[] { msg });
            }
            else
            {
                consoleTextBox.Text += $"[{DateTime.Now.ToString("HH:mm:ss")}]: {msg}\n";
            }
        }
    }
}
