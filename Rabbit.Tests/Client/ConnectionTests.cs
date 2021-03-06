﻿using System.Text;
using System.Linq;
using Rabbit.Client;
using Xunit;
using Xunit.Abstractions;

namespace Rabbit.Tests.Client
{
    public class ConnectionTests
    {
        readonly Connection conn;
        readonly SocketFake socket;

        public ConnectionTests()
        {
            this.socket = new SocketFake();
            this.conn = new Connection("whatever", 2626, socket);
        }

        private void GivenSocketReceive(string msg)
        {
            this.socket.ReceiveData.Add(msg);
        }


        void ThenSocketSended(byte[] data)
        {
            Assert.Contains(data, this.socket.SendedData);
        }

        [Fact]
        public void Send_ShouldSucceed()
        {
            this.conn.Send("first message");

            const string sendedMsg = "first message\n";
            byte[] sendedData = Encoding.ASCII.GetBytes(sendedMsg);
            this.ThenSocketSended(sendedData);
        }

        [Fact]
        public void ReceivedInscriptionok_ShouldSucced()
        {
            this.GivenSocketReceive("Inscription OK");

            var msg = this.conn.Receive().First();

            Assert.Equal(MessageType.InscriptionOk, msg.Type);
            Assert.Equal("Inscription OK", msg.Data);
        }

    }
}
