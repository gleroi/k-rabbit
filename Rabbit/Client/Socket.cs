﻿using System;
using System.Net.Sockets;

namespace Rabbit.Client
{
    public interface ISocket
    {
        void Connect(string host, int port);
        int Send(byte[] data);
        int Receive(byte[] data);
    }

    class SocketWrapper : ISocket
    {
        Socket server;

        public void Connect(string host, int port)
        {
            Log.Write("Creating socket");

            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Log.Write("IP is: {0}", host);

            try
            {
                this.server.Connect(host, port);
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

        public int Send(byte[] data)
        {
            return this.server.Send(data);
        }

        public int Receive(byte[] data)
        {
            return this.server.Receive(data);
        }
    }

}
