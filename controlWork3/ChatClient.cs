using System;
using System.Net;
using System.Net.Sockets;

namespace ChatApp
{
    public class ChatClient
    {
        private readonly IPAddress ip;
        private readonly int port;

        public ChatClient(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        /// <summary>
        /// Connects to a chat server and starts a chat session.
        /// </summary>
        public void Start()
        {
            var client = new TcpClient();
            client.Connect(ip, port);

            Console.WriteLine("Connected to server");

            var session = new ChatSession(client, Console.In, Console.Out);
            session.RunAsync().Wait();
        }
    }
}