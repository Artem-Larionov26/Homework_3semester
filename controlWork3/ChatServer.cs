using System;
using System.Net;
using System.Net.Sockets;

namespace ChatApp
{
    public class ChatServer
    {
        private readonly int port;

        public ChatServer(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// Starts a TCP chat server and waits for a single client connection.
        /// </summary>
        public void Start()
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"Server started on port {port}");

            var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");

            var session = new ChatSession(client, Console.In, Console.Out);
            session.RunAsync().Wait();

            listener.Stop();
        }
    }
}