// <copyright file="Server.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyNetworkingProject
{
    public class Server
    {
        private readonly int port;
        private TcpListener listener;

        public Server(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine($"The server is running on the port {port}. Waiting for connections...");

            var serverThread = new Thread(ListenForClients);
            serverThread.Start();
        }

        private void ListenForClients()
        {
            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("The client has connected!");

                    var clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Message from the client: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in client processing: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
