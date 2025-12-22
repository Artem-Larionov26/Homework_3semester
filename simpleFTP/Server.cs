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
    /// <summary>
    /// Simple FTP-like server that supports two commands:
    /// 1 - list directory contents
    /// 2 - download file
    /// Communication is performed over TCP using a custom binary protocol.
    /// </summary>
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

            Console.WriteLine($"Server started on port {port}");

            var serverThread = new Thread(ListenForClients);
            serverThread.Start();
        }

        /// <summary>
        /// Continuously accepts incoming TCP connections.
        /// Each client is processed in a separate thread.
        /// </summary>
        private void ListenForClients()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected");

                var clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

                string? request = reader.ReadLine();
                if (string.IsNullOrEmpty(request))
                {
                    return;
                }

                Console.WriteLine($"Request: {request}");

                string[] parts = request.Split(' ', 2);
                if (parts.Length != 2)
                {
                    writer.Write(-1L);
                    return;
                }

                string command = parts[0];
                string path = parts[1];

                switch (command)
                {
                    case "1":
                        ProcessListCommand(path, writer);
                        break;

                    case "2":
                        ProcessGetCommand(path, writer);
                        break;

                    default:
                        writer.Write(-1L);
                        break;
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

        /// <summary>
        /// Sends directory listing to the client.
        /// Protocol:
        /// Int64 count
        /// (string name, bool isDirectory) * count
        /// </summary>
        private void ProcessListCommand(string path, BinaryWriter writer)
        {
            if (!Directory.Exists(path))
            {
                writer.Write(-1L);
                return;
            }

            var entries = Directory.GetFileSystemEntries(path);
            writer.Write((long)entries.Length);

            foreach (var entry in entries)
            {
                string name = Path.GetFileName(entry);
                bool isDir = Directory.Exists(entry);

                writer.Write(name);
                writer.Write(isDir);
            }

            Console.WriteLine($"List sent for {path}");
        }

        /// <summary>
        /// Sends file content to the client.
        /// Protocol:
        /// Int64 size
        /// byte[size] content
        /// </summary>

        private void ProcessGetCommand(string path, BinaryWriter writer)
        {
            if (!File.Exists(path))
            {
                writer.Write(-1L);
                return;
            }

            byte[] content = File.ReadAllBytes(path);
            writer.Write((long)content.Length);
            writer.Write(content);

            Console.WriteLine($"File sent: {path} ({content.Length} bytes)");
        }
    }
}