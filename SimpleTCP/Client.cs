// <copyright file="Client.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleFTP
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

        public void SendCommand(string command)
        {
            try
            {
                using var client = new TcpClient(host, port);
                using var stream = client.GetStream();
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

                writer.WriteLine(command);
                Console.WriteLine($"Shipped: {command}");

                if (command.StartsWith("1"))
                {
                    string response = ReadResponseString(reader);
                    Console.WriteLine($"Server response: {response}");
                }
                else if (command.StartsWith("2"))
                {
                    long size = ReadFileSize(reader);
                    if (size == -1)
                    {
                        Console.WriteLine("Server response: file not found.");
                        return;
                    }

                    byte[] data = reader.ReadBytes((int)size);
                    Console.WriteLine($"Received file ({size} bytes):");
                    Console.WriteLine(Encoding.UTF8.GetString(data));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }

        private string ReadResponseString(BinaryReader reader)
        {
            var sb = new StringBuilder();
            char c;
            while ((c = reader.ReadChar()) != '\n')
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        private long ReadFileSize(BinaryReader reader)
        {
            var sb = new StringBuilder();
            char c;
            while (true)
            {
                c = reader.ReadChar();
                if (c == ' ' || c == '\n')
                {
                    break;
                }
                sb.Append(c);
            }
            return long.Parse(sb.ToString());
        }
    }
}