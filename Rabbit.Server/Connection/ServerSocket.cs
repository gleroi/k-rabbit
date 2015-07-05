using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.Client;

namespace Rabbit.Server.Connection
{
    class ServerSocket : ISocket
    {
        #region ISocket Members

        public void Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
