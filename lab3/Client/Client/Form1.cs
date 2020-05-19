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
        private List<Thread> _threads = new List<Thread>();

        public Form1()
        {
            InitializeComponent();
        }

        private static void applicationRunProc(object state)
        {
            GUI.ClientGUI client = (GUI.ClientGUI)state;
            client.Start();
            Application.Run(client);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string ip = ipCheck(ipTextBox.Text);
            int port = portCheck(portTextBox.Text);

            if (ip != "" && port != -1)
            {
                Thread thread = new Thread(applicationRunProc);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = false;
                thread.Start(new GUI.ClientGUI(ip, port, AddLog));
                //_threads.Add(new Thread(() =>
                //{
                //    SocketLibTester.SocketHelpers.Client client = new SocketLibTester.SocketHelpers.Client(ip, port);
                //    ClientGUI clientGUI = new ClientGUI(client, AddLog);
                //    clientGUI.Start();
                //    //clientGUI.Show();
                //    //clientGUI.ShowDialog();
                //}));
                //_threads.Last().Start();
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
