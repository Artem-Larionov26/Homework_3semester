// <copyright file="Client.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Net.Sockets;
using System.Text;

namespace MyNetworkingProject
{
    public class Client
    {
        private readonly string host;
        private readonly int port;

        public Client(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void SendMessage(string message)
        {
            try
            {
                using var client = new TcpClient(host, port);
                using var stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Console.WriteLine($"A message has been sent: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }
    }
}
