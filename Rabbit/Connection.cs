using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rabbit
{

    class Connection
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        private ISocket server;

        public Connection(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public void Connect()
        {
            Log.Write("Connection to {0}:{1}", this.Host, this.Port);

            this.server.Connect(this.Host, this.Port);
        }

        public Message Receive()
        {
            Log.Write("Reading message from server");

            var data = ReadData();
            Message msg;

            if (data.StartsWith("worldstate::"))
            {
                msg = new Message(MessageType.WorkState, data);
            }
            else if (data.StartsWith("game over"))
            {
                msg = new Message(MessageType.GameOver, data);
            }
            else if (data.StartsWith("Inscription OK"))
            {
                msg = new Message(MessageType.InscriptionOk, data);
            }
            else if (data.StartsWith("Inscription KO"))
            {
                msg = new Message(MessageType.InscriptionKo, data);
            }
            else
            {
                Log.Write("Unknown message: " + data);
                throw new InvalidOperationException("message inconnu " + data);
            }

            Log.Write("Message Recu: " + msg.Type.ToString());
            return msg;
        }

        private string ReadData()
        {
            const int BUFFER_LEN = 1024;
            var recvData = new byte[BUFFER_LEN];
            string data = string.Empty;
            int recvLen = 0;

            Log.Write("Reading data");

            try
            {

                do
                {
                    recvLen = this.server.Receive(recvData);
                    data += Encoding.ASCII.GetString(recvData, 0, recvLen);
                }
                while (recvLen > 0);
            }
            catch (Exception ex)
            {
                Log.Write("Exception while receiving message");
                Log.Write(ex.ToString());
            }

            Log.Write(data.Length + " bytes read");

            return data;
        }

        public void Send(string msg)
        {
            var data = Encoding.ASCII.GetBytes(msg);
            try
            {
                Log.Write("Sending " + data.Length + " bytes");

                int sent = this.server.Send(data);

                Log.Write("Sended " + data.Length + " bytes");
            }
            catch (Exception ex)
            {
                Log.Write("Exception while sending: " + msg);
                Log.Write(ex.ToString());
            }
        }
    }
}
