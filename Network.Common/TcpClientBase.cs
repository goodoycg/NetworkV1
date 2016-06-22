using System;
using System.Collections.Generic;
using System.Net.Sockets;
 
namespace Network.Common
{
    public class TcpClientBase : TcpClient
    {
        public event TcpServerBase.OnMessageEventHandler OnMessage;
        public event TcpServerBase.OnErrorEventHandler OnError;

        private byte[] Buffer;
        private List<byte> Container = new List<byte>();

        public string Host { get; set; }
        public int Port { get; private set; }
        public bool AsyncExecution { get; set; }
        public TcpClientBase(string host, int port, int bufferSize = 320000, bool asyncExecution = true)
            : base(host, port)
        {
            this.Host = host;
            this.Port = port;
            this.AsyncExecution = asyncExecution;
            Buffer = new byte[bufferSize];
            this.GetStream().BeginRead(Buffer, 0, Buffer.Length, ClientRead, Buffer);
        }

        private void ClientRead(IAsyncResult h)
        {
            var stream = this.GetStream();
            TcpServerBase.ExecuteRead(stream, h, Buffer, Container, AsyncExecution, OnMessage, OnError);
            try
            {
                stream.BeginRead(Buffer, 0, Buffer.Length, ClientRead, Buffer);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(stream, ex);
                }
            }
        }

        public void SendMessage(string msg, Action<Exception> error = null)
        {
            TcpServerBase.SendMessage(this.GetStream(), msg, error);
        }

        
    }
}