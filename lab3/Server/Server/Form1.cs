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
using crypto_test;
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
                    new Command("encrypt", (state, parts) =>
                    {
                        string pass = "";
                        string text = "";
                        bool isFile = false;
                        for (int i = 1; i < parts.Length; ++i)
                        {
                            if (parts[i] == "-f")
                            {
                                isFile = true;
                            }
                            else if (parts[i] == "-p")
                            {
                                if (i + 1 == parts.Length)
                                {
                                    return "Wrong cmd: use encrypt [-p] <pass> <message>";
                                }
                                else
                                {
                                    pass = parts[i + 1];
                                    ++i;
                                }
                            }
                            else if (pass != "")
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int j = i; j < parts.Length; ++j)
                                {
                                    sb.Append(parts[j]);
                                }
                                text = sb.ToString();
                                break;
                            }
                        }
                        if (pass != "" && text != "")
                        {
                            MD5Hash hasher = new MD5Hash();
                            pass = hasher.GetHash(pass, true);
                            AES aes = new AES();
                            text = aes.Encrypt(text, pass, !isFile);
                            return text;
                        }
                        else
                        {
                            return "Wrong cmd: use encrypt [-p] <pass> <message>";
                        }
                    }),
                    new Command("decrypt", (state, parts) =>
                    {
                        string pass = "";
                        string text = "";
                        bool isFile = false;
                        for (int i = 1; i < parts.Length; ++i)
                        {
                            if (parts[i] == "-f")
                            {
                                isFile = true;
                            }
                            else if (parts[i] == "-p")
                            {
                                if (i + 1 == parts.Length)
                                {
                                    return "Wrong cmd: use encrypt [-p] <pass> <message>";
                                }
                                else
                                {
                                    pass = parts[i + 1];
                                    ++i;
                                }
                            }
                            else if (pass != "")
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int j = i; j < parts.Length; ++j)
                                {
                                    sb.Append(parts[j]);
                                }
                                text = sb.ToString();
                                break;
                            }
                        }
                        if (pass != "" && text != "")
                        {
                            MD5Hash hasher = new MD5Hash();
                            pass = hasher.GetHash(pass, true);
                            AES aes = new AES();
                            text = aes.Decrypt(text, pass);
                            return text;
                        }
                        else
                        {
                            return "Wrong cmd: use encrypt [-p] <pass> <message>";
                        }

                    }),
                    new Command("help", (state, parts) =>
                    {
                        string help = "hello <n>, bye <n>, [encrypt | decrypt] -p <pass> <text>";
                        return help;
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
