using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Network.Common;


namespace Network.Server
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {//http://bbs.csdn.net/topics/390987992             SP1234
            InitializeComponent();
        }
        private TcpServerBase server;
        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            server = new TcpServerBase(Convert.ToInt32(this.txtPort.Text.Trim()));
            server.OnOpen -= Server_OnOpen;
            server.OnMessage -= Server_OnMessage;
            server.OnError -= Server_OnError;

            server.OnError += Server_OnError;
            server.OnOpen += Server_OnOpen;
            server.OnMessage += Server_OnMessage;   
        }

        private void Server_OnError(System.Net.Sockets.NetworkStream stream, Exception ex)
        {
            if (DictClient.Count == 0)
            {
                return;
            }
            var entry = DictClient.Where(e => e.Value == stream).First();            
            this.ChangeItem(entry.Key, false);
            DictClient.Remove(entry.Key);
        }
        private void Server_OnOpen(System.Net.IPEndPoint endpoint, System.Net.Sockets.NetworkStream stream)
        {//客户端连接上服务器
            this.ChangeItem(endpoint.Address.ToString(),true);
            DictClient.Add(endpoint.Address.ToString(), stream);//客户端对应的流，保存起来，用于与这个客户端进行通讯。
            
        }
        private void Server_OnMessage(System.Net.Sockets.NetworkStream stream, string message)
        {
            ShowText(this.txtMsg, message);
        }
        delegate void AddText(TextBox txtBox, string text);
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
        private Dictionary<string, NetworkStream> DictClient = new Dictionary<string, NetworkStream>();


        delegate void ItemChanged(string text,bool IsAdd);
        private void ChangeItem(string text, bool IsAdd)
        {
            if (this.lbSocketOnline.InvokeRequired)
            {
                ItemChanged stcb = new ItemChanged(ChangeItem);
                this.Invoke(stcb, new object[] { text, IsAdd });
            }
            else
            {
                if (IsAdd)
                {
                    this.lbSocketOnline.Items.Add(text);
                }
                else
                {
                    this.lbSocketOnline.Items.Remove(text);
                }
            }
        }        
        

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            if (this.lbSocketOnline.SelectedIndex < 0)
            {
                return;
            }        
            SendMsg(DictClient[this.lbSocketOnline.SelectedItem.ToString()]);
        }

        private void btnSendToAll_Click(object sender, EventArgs e)
        {
            DictClient.Values.ToList<NetworkStream>().ForEach(s => SendMsg(s));
        }
        private void SendMsg(NetworkStream stream)
        {
            TcpServerBase.SendMessage(stream, this.txtSendMsg.Text.Trim());
        }
        private void SendMsg(NetworkStream stream, string Text)
        {
            TcpServerBase.SendMessage(stream, Text);
        }
    }
}
