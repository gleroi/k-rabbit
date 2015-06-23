using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                else if (data.StartsWith("action OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    msg = new Message(MessageType.ActionOk, data);
                }

                else
                {
                    Log.Write("Unknown message: " + data);
                    continue;
                }

                Log.Write("Message Recu: " + msg.Type);

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