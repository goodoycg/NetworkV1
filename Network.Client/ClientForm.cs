using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Network.Common;

namespace Network.Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {//http://bbs.csdn.net/topics/390987992             SP1234
            InitializeComponent();
        }
        private TcpClientBase m_client;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            m_client = new TcpClientBase(this.txtIP.Text.Trim(), Convert.ToInt32(this.txtPort.Text.Trim()));
            m_client.OnError -= Client_OnError;
            m_client.OnMessage -= Client_OnMessage;

            m_client.OnError += Client_OnError;
            m_client.OnMessage += Client_OnMessage;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_client == null)
            {
                return;
            }
            m_client.SendMessage(this.txtSendMsg.Text.Trim()); 
        }
        private void Client_OnMessage(System.Net.Sockets.NetworkStream stream, string message)
        {
            ShowText(this.txtMsg, message);
        }

        private void Client_OnError(System.Net.Sockets.NetworkStream stream, Exception ex)
        {
            ShowText(this.txtMsg, ex.Message);
        }

        delegate void AddText(TextBox txtBox,string text);
        private void ShowText(TextBox txtBox, string text)
        {
            if (txtBox.InvokeRequired)
            {
                AddText stcb = new AddText(ShowText);
                this.Invoke(stcb, new object[] { txtBox, text });
            }
            else
            {
                txtBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text + "\r\n" + txtBox.Text;
            }
        }
    }
}
