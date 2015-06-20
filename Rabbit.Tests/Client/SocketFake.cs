using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.Client;
using Xunit;

namespace Rabbit.Tests.Client
{
    class SocketFake : ISocket
    {
        public List<byte[]> SendedData = new List<byte[]>();

        public List<string> ReceiveData = new List<string>();
        int currentReceivedData = 0;

        public void Connect(string host, int port)
        {
        }

        public int Send(byte[] data)
        {
            this.SendedData.Add(data);
            return data.Length;
        }

        public int Receive(byte[] data)
        {
            if (this.currentReceivedData < this.ReceiveData.Count)
            {
                byte[] recvData = Encoding.ASCII.GetBytes(this.ReceiveData[this.currentReceivedData]);
                this.currentReceivedData += 1;
                
                recvData.CopyTo(data, 0);
                return recvData.Length;
            }
            return 0;
        }
    }
}
