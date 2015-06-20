using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.Client;

namespace Rabbit.Tests.Client
{
    class SocketFake : ISocket
    {
        public void Connect(string host, int port)
        {
        }

        public int Send(byte[] data)
        {
            return 0;
        }

        public int Receive(byte[] data)
        {
            return 0;
        }
    }
}
