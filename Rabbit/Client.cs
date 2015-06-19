using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Rabbit
{
    // teamId: 170
    // secret: mUrUs2

    // host: battle.gate.vm.gate.erdf.fr
    // port: 2026

    enum Direction
    {
        N,
        S,
        E,
        O
    }

    enum MessageType {
        InscriptionOk,
        InscriptionKo,
        WorkState,
        GameOver,
    }

    struct Message
    {
        public MessageType Type { get; private set; }
        public string Data { get; private set; }

        public Message(MessageType type, string data)
            : this()
        {
            this.Type = type;
            this.Data = data;
        }
    }

    class Connection
    {
        public string Host { get; private set; }
        public int Port { get; private set; }

        string Secret { get; private set; }
        int GameId { get; private set; }
        int TeamId { get; private set; }

        private Socket server;

        public Connection(string secret, int gameId, int teamId)
        {
            this.Secret = secret;
            this.GameId = gameId;
            this.TeamId = teamId;
        }

        private void CreateSocket()
        {
            Log.Write("Creating socket");

            var hostEntry = Dns.GetHostEntry(this.Host);
            var ip = hostEntry.AddressList.First(addr => addr.AddressFamily == AddressFamily.InterNetwork);
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Log.Write("IP is: {0}", ip);

            try
            {
                this.server.Connect(ip, this.Port);
                if (!this.server.Connected)
                {
                    throw new InvalidOperationException("Could not connect to server");
                }
            }
            catch (Exception ex)
            {
                Log.Write("Exception occured while connecting");
                Log.Write(ex.ToString());
                throw;
            }
        }

        public void Connect()
        {
            Log.Write("Connection to {0}:{1}", this.Host, this.Port);

            CreateSocket();

            SendInscripton();

            WaitInscriptionAck();
        }

        private void WaitInscriptionAck()
        {
            Log.Write("Waiting for inscripton acknowloedgement");

            var msg = ReadMsg();
        }

        private Message ReadMsg()
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

            try
            {

                do
                {
                    recvLen = this.server.Receive(recvData, BUFFER_LEN, SocketFlags.None);
                    data += Encoding.ASCII.GetString(recvData, 0, recvLen);
                }
                while (recvLen > 0);
            }
            catch (Exception ex)
            {
                Log.Write("Exception while receiving message");
                Log.Write(ex.ToString());
            }
            return data;
        }

        private void SendInscripton()
        {
            Log.Write("Sending inscription");

            var msg = this.GetInscripitionMessage();

            Log.Write("Inscription is " + msg);

            this.Send(msg);
        }

        private string GetInscripitionMessage()
        {
            return this.Secret + "%%inscription::" + this.GameId + ";" + this.TeamId;
        }

        public void SendMove(int round, Direction direction)
        {
            Log.Write("Sending move");

            var msg = GetMoveMessage(round, direction);

            Log.Write("Move is " + msg);

            this.Send(msg);
        }

        private string GetMoveMessage(int round, Direction direction)
        {
            string dirStr = null;
            switch (direction)
            {
                case Direction.N:
                    dirStr = "N";
                    break;
                case Direction.S:
                    dirStr = "S";
                    break;
                case Direction.E:
                    dirStr = "E";
                    break;
                case Direction.O:
                    dirStr = "O";
                    break;
            }
            return this.Secret + "%%action::" + this.TeamId + ";" + this.GameId + ";" + round + ";" + dirStr;
        }

        private void Send(string msg)
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

    class Client
    {
        public Client(Connection conn)
        {

        }
    }
}
