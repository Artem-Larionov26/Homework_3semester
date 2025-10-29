// <copyright file="Server.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleFTP
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
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                string? request = reader.ReadLine();
                if (string.IsNullOrEmpty(request))
                {
                    return;
                }

                Console.WriteLine($"Request from the client: {request}");

                string[] parts = request.Split(' ', 2);
                if (parts.Length < 2)
                {
                    writer.WriteLine("-1");
                    writer.Flush();
                    return;
                }

                string command = parts[0];
                string path = parts[1];

                if (command == "1")
                {
                    ProcessListCommand(path, writer);
                }
                else if (command == "2")
                {
                    ProcessGetCommand(path, writer);
                }
                else
                {
                    writer.WriteLine("-1");
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private void ProcessListCommand(string path, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                writer.WriteLine("-1");
                writer.Flush();
                Console.WriteLine($"The directory '{path}' does not exist.");
                return;
            }

            var entries = Directory.GetFileSystemEntries(path);
            var sb = new StringBuilder();
            sb.Append(entries.Length);

            foreach (var entry in entries)
            {
                string name = Path.GetFileName(entry);
                bool isDir = Directory.Exists(entry);
                sb.Append($" {name} {isDir.ToString().ToLower()}");
            }

            sb.Append('\n');
            writer.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            writer.Flush();
            Console.WriteLine($"A list has been sent for '{path}'");
        }

        private void ProcessGetCommand(string path, BinaryWriter writer)
        {
            if (!File.Exists(path))
            {
                writer.Write(Encoding.UTF8.GetBytes("-1\n"));
                writer.Flush();
                Console.WriteLine($"The file '{path}' does not exist.");
                return;
            }

            byte[] content = File.ReadAllBytes(path);
            long size = content.Length;

            writer.Write(Encoding.UTF8.GetBytes($"{size} "));
            writer.Flush();

            writer.Write(content);
            writer.Flush();

            Console.WriteLine($"The file '{path}' has been sent ({size} bytes).");
        }
    }
}