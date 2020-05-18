using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private delegate void SafeCallDelegate(string msg);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string ip = ipCheck(ipTextBox.Text);
            int port = portCheck(portTextBox.Text);

            if (ip != "" && port != -1)
            {
                Thread thread = new Thread(() =>
                {
                    SocketLibTester.SocketHelpers.Client client = new SocketLibTester.SocketHelpers.Client(ip, port);
                    ClientGUI clientGUI = new ClientGUI(client, AddLog);
                    clientGUI.Start();
                    //clientGUI.Show();
                });
                thread.Start();
            }

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

    }
}
