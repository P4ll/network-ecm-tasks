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

namespace Client
{
    public partial class ClientGUI : Form
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private SocketLibTester.SocketHelpers.Client _client;

        public ClientGUI(string ip, int port, SocketLibTester.SocketHelpers.Client.AddLogDelegate addMainConsole)
        {
            InitializeComponent();
            Ip = ip;
            Port = port;
            labelIP.Text = Ip;
            portLabel.Text = Port.ToString();
            _client = new SocketLibTester.SocketHelpers.Client(Ip, Port);
            _client.AddLog = AddClientLog;
            _client.AddMainLog = addMainConsole;
        }

        public ClientGUI(SocketLibTester.SocketHelpers.Client client, SocketLibTester.SocketHelpers.Client.AddLogDelegate addMainConsole)
        {
            InitializeComponent();
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
            _client.SendMessage(inputTextBox.Text);
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
            if (consoleTextBox.InvokeRequired)
            {
                SocketLibTester.SocketHelpers.Client.AddLogDelegate d = new SocketLibTester.SocketHelpers.Client.AddLogDelegate(AddClientLog);
                consoleTextBox.Invoke(d, new object[] { msg });
            }
            else
            {
                consoleTextBox.Text += $"[{DateTime.Now.ToString("HH:mm:ss")}]: {msg}\n";
            }
        }
    }
}
