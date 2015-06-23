﻿using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Client
{
    internal class Client
    {
        private readonly string Secret;
        private readonly int GameId;
        private readonly int TeamId; 

        private readonly Connection Server;

        public Client(Connection conn, string secret, int gameId, int teamId)
        {
            this.Secret = secret;
            this.GameId = gameId;
            this.TeamId = teamId;

            this.Server = conn;
        }

        public void Connect()
        {
            this.Server.Connect();

            SendInscripton();

            WaitInscriptionAck();
        }

        private void SendInscripton()
        {
            Log.Write("Sending inscription");

            var msg = this.GetInscripitionMessage();

            Log.Write("Inscription is " + msg);

            this.Server.Send(msg);
        }

        private string GetInscripitionMessage()
        {
            return this.Secret + "%%inscription::" + this.GameId + ";" + this.TeamId;
        }

        private void WaitInscriptionAck()
        {
            Log.Write("Waiting for inscripton acknowledgement");

            var msg = this.Server.Receive().First();

            switch (msg.Type)
            {
                case MessageType.InscriptionKo:
                case MessageType.GameOver:
                    throw new ConnectionException("Connection failed: " + msg.Data);
                default:
                    Log.Write("Connection: receveived: " + msg.Data);
                    return;
            }
        }

        public IEnumerable<Message> WaitMessages()
        {
            Log.Write("Waiting for message");

            foreach (var msg in this.Server.Receive())
            {
                Log.Write("Message received :" + msg.Data);
                yield return msg;
            }
        }

        public void SendMove(int round, Direction direction)
        {
            Log.Write("Sending move");

            var msg = GetMoveMessage(round, direction);

            Log.Write("Move is " + msg);

            this.Server.Send(msg);
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
    }
}