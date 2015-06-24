using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit.Client
{
    internal class Connection
    {
        public readonly string Host;
        public readonly int Port;
        private readonly ISocket server;

        public Connection(string host, int port, ISocket socket)
        {
            this.Host = host;
            this.Port = port;
            this.server = socket;
        }

        public void Connect()
        {
            Log.Debug("Connection to {0}:{1}", this.Host, this.Port);

            this.server.Connect(this.Host, this.Port);
        }

        public IEnumerable<Message> Receive()
        {
            Log.Debug("Reading message from server");

            var lines = this.ReadData();

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
                else if (data.StartsWith("action OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    msg = new Message(MessageType.ActionOk, data);
                }

                else
                {
                    Log.Error("Unknown message: " + data);
                    continue;
                }

                Log.Debug("Message Recu: " + msg.Type);

                yield return msg;
            }
        }

        private IEnumerable<string> ReadData()
        {
            const int BUFFER_LEN = 1024;
            var recvData = new byte[BUFFER_LEN];

            Log.Debug("Reading data");

            while (true)
            {
                var recvLen = 0;
                try
                {
                    recvLen = this.server.Receive(recvData);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception while receiving message");
                    Log.Error(ex.ToString());
                    throw;
                }

                int start = 0;
                for (int i = 0; i < recvLen; i++)
                {
                    var c = recvData[i];
                    if (c == '\n')
                    {
                        if (i > start)
                        {
                            var line = Encoding.ASCII.GetString(recvData, start, i - start);
                            yield return line;
                        }
                        start = i + 1;
                    }
                }
            }
        }

        public void Send(string msg)
        {
            var data = Encoding.ASCII.GetBytes(msg + "\n");
            try
            {
                Log.Debug("Sending " + data.Length + " bytes");

                int sent = this.server.Send(data);

                Log.Debug("Sended " + sent + " bytes");
            }
            catch (Exception ex)
            {
                Log.Error("Exception while sending: " + msg);
                Log.Error(ex.ToString());
            }
        }
    }
}