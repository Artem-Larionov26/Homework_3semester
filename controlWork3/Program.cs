using System;
using System.Net;

namespace ChatApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && int.TryParse(args[0], out int port))
            {
                RunServer(port);
            }
            else if (args.Length == 2 &&
                     IPAddress.TryParse(args[0], out var ip) &&
                     int.TryParse(args[1], out port))
            {
                RunClient(ip, port);
            }
            else
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  ChatApp <port>");
                Console.WriteLine("  ChatApp <ip> <port>");
            }
        }

        static void RunServer(int port)
        {
            var server = new ChatServer(port);
            server.Start();
        }

        static void RunClient(IPAddress ip, int port)
        {
            var client = new ChatClient(ip, port);
            client.Start();
        }
    }
}