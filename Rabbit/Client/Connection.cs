using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rabbit.Client
{

    class Connection
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        private readonly ISocket server;

        public Connection(string host, int port, ISocket socket)
        {
            this.Host = host;
            this.Port = port;
            this.server = socket;
        }

        public void Connect()
        {
            Log.Write("Connection to {0}:{1}", this.Host, this.Port);

            this.server.Connect(this.Host, this.Port);
        }

        public IEnumerable<Message> Receive()
        {
            Log.Write("Reading message from server");

            var lines = ReadData();

            foreach (var data in lines)
            {
                Message msg;

                if (data.StartsWith("worldstate::"))
                {
                    msg = new Message(MessageType.WorkState, data);
                }
                else if (data.StartsWith("game over"))
                {
                    msg = new Message(MessageType.GameOver, data);
                }
                else if (data.StartsWith("inscription OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    msg = new Message(MessageType.InscriptionOk, data);
                }
                else if (data.StartsWith("inscription KO", StringComparison.InvariantCultureIgnoreCase))
                {
                    msg = new Message(MessageType.InscriptionKo, data);
                }
                else
                {
                    Log.Write("Unknown message: " + data);
                    continue;
                }

                Log.Write("Message Recu: " + msg.Type.ToString());

                yield return msg;
            }
        }

        private IEnumerable<string> ReadData()
        {
            const int BUFFER_LEN = 1024;
            var recvData = new byte[BUFFER_LEN];
            int recvLen = 0;

            Log.Write("Reading data");

            while (true)
            {
                try
                {
                    recvLen = this.server.Receive(recvData);
                }
                catch (Exception ex)
                {
                    Log.Write("Exception while receiving message");
                    Log.Write(ex.ToString());
                    throw;
                }

                var data = Encoding.ASCII.GetString(recvData, 0, recvLen);
                var lines = data.Split('\n');
                foreach (var line in lines)
                {
                    yield return line;
                }
            }
        }

        public void Send(string msg)
        {
            var data = Encoding.ASCII.GetBytes(msg + "\n");
            try
            {
                Log.Write("Sending " + data.Length + " bytes");

                int sent = this.server.Send(data);

                Log.Write("Sended " + sent + " bytes");
            }
            catch (Exception ex)
            {
                Log.Write("Exception while sending: " + msg);
                Log.Write(ex.ToString());
            }
        }
    }
}
