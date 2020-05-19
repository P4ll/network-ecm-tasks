using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketLibTester.SocketHelpers;

namespace SoloClient
{
    public partial class ClientGUI : Form
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private delegate void SafeCallDelegate(string msg);
        private delegate void SafeCloseDelegate();

        private SocketLibTester.SocketHelpers.Client _client;

        private bool _isConnected = false;

        public ClientGUI()
        {
            InitializeComponent();
        }

        public ClientGUI(string ip, int port, SocketLibTester.SocketHelpers.Client.AddLogDelegate addMainConsole)
        {
            InitializeComponent();
            Ip = ip;
            Port = port;
            _client = new SocketLibTester.SocketHelpers.Client(Ip, Port);
            _client.AddLog = AddClientLog;
            _client.AddMainLog = addMainConsole;
        }

        public ClientGUI(SocketLibTester.SocketHelpers.Client client, SocketLibTester.SocketHelpers.Client.AddLogDelegate addMainConsole)
        {
            InitializeComponent();
            Ip = client.Ip;
            Port = client.Port;
            _client = client;
            _client.AddMainLog = addMainConsole;
            _client.AddLog = AddClientLog;
        }

        public void Start()
        {
            _client.Start(this);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (_isConnected)
                _client.SendMessage(inputTextBox.Text, inputTextBox);
            else
                AddClientLog("Вы не подключены к серверу");
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEnter_Click(sender, e);
            }
        }

        public void AddClientLog(string msg)
        {
            if (msg == "") return;

            if (consoleTextBox.InvokeRequired)
            {
                SafeCallDelegate d = new SafeCallDelegate(AddClientLog);
                consoleTextBox.Invoke(d, new object[] { msg });
            }
            else
            {
                consoleTextBox.Text += $"[{DateTime.Now.ToString("HH:mm:ss")}]: {msg}\n";
            }
        }

        public void SafeClose()
        {
            if (this.InvokeRequired)
            {
                SafeCloseDelegate d = new SafeCloseDelegate(SafeClose);
                this.Invoke(d);
            }
            else
            {
                this.Close();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                string ip = ipCheck(ipTextBox.Text);
                int port = portCheck(portTextBox.Text);
                _client = new Client(ip, port);
                _client.AddLog = AddClientLog;
                _client.AddMainLog = AddClientLog;
                Start();
            }
            else
            {
                _client.Stop();
            }
        }

        private string ipCheck(string ip)
        {
            for (int i = 0; i < ip.Length; ++i)
            {
                if (!Char.IsDigit(ip[i]) && ip[i] != '.')
                {
                    AddClientLog("Ошибка в адресе");
                    return "";
                }
            }

            string[] parts = ip.Split('.');
            foreach (string part in parts)
            {
                int c = -1;
                if (!Int32.TryParse(part, out c) || c < 0 || c > 255)
                {
                    AddClientLog("Ошибка в адресе");
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
                AddClientLog("Ошибка в значении порта");
                return -1;
            }
            return portInt;
        }

        public void SafeChangeConnection()
        {
            if (btnConnect.InvokeRequired)
            {
                SafeCloseDelegate d = new SafeCloseDelegate(SafeChangeConnection);
                btnConnect.Invoke(d);
            }
            else
            {
                if (_isConnected == true)
                {
                    _isConnected = false;
                    btnConnect.Text = "Подключиться";
                }
                else
                {
                    _isConnected = true;
                    btnConnect.Text = "Отключиться";
                }
            }
        }
    }
}
