using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Rabbit.Client;
using Xunit;

namespace Rabbit.Tests.Client
{
    public class ConnectionTests
    {
        readonly Connection conn;
        readonly Mock<ISocket> socket;
        public ConnectionTests()
        {
            this.socket = new Mock<ISocket>();
            this.conn = new Connection("whatever", 2626, socket.Object);
        }

        [Fact]
        public void Send_ShouldSucceed()
        {
            const string sendedMsg = "first message";
            this.conn.Send("first message");

            byte[] sendedData = Encoding.ASCII.GetBytes(sendedMsg);
            this.socket.Verify(socket => socket.Send(sendedData), Times.Once);
        }

        [Fact]
        public void ReceivedEnmpty_ShouldThrowMessageInconnu()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => this.conn.Receive());

            Assert.Equal("message inconnu: ", ex.Message);
            this.socket.Verify(socket => socket.Receive(It.IsAny<byte[]>()), Times.Once);
        }

    }
}
