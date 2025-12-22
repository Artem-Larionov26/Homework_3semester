// <copyright file="Client.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleFTP
{
    /// <summary>
    /// Client for SimpleFTP server.
    /// Provides methods for sending commands and receiving responses.
    /// </summary>
    public class Client
    {
        private readonly string host;
        private readonly int port;

        public Client(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Sends LIST command to the server and prints the result.
        /// </summary>
        public void SendList(string path)
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            writer.WriteLine($"1 {path}");

            long size = reader.ReadInt64();
            if (size == -1)
            {
                Console.WriteLine("Directory does not exist");
                return;
            }

            Console.WriteLine($"Entries count: {size}");

            for (int i = 0; i < size; i++)
            {
                string name = reader.ReadString();
                bool isDir = reader.ReadBoolean();

                Console.WriteLine($"{name} {(isDir ? "[DIR]" : "[FILE]")}");
            }
        }

        /// <summary>
        /// Sends GET command to the server and prints file content.
        /// </summary>
        public void SendGet(string path)
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            writer.WriteLine($"2 {path}");

            long size = reader.ReadInt64();
            if (size == -1)
            {
                Console.WriteLine("File does not exist");
                return;
            }

            byte[] content = reader.ReadBytes((int)size);

            Console.WriteLine($"File received ({size} bytes)");
            Console.WriteLine(Encoding.UTF8.GetString(content));
        }

        /// <summary>
        /// Sends LIST command and returns directory entries (used in tests).
        /// </summary>
        public DirectoryEntry[] GetList(string path)
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            writer.WriteLine($"1 {path}");

            long size = reader.ReadInt64();
            if (size == -1)
            {
                throw new DirectoryNotFoundException(path);
            }

            var result = new DirectoryEntry[size];
            for (int i = 0; i < size; i++)
            {
                string name = reader.ReadString();
                bool isDir = reader.ReadBoolean();
                result[i] = new DirectoryEntry(name, isDir);
            }

            return result;
        }

        /// <summary>
        /// Sends GET command and returns file content as byte array (used in tests).
        /// </summary>
        public byte[] GetFile(string path)
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            writer.WriteLine($"2 {path}");

            long size = reader.ReadInt64();
            if (size == -1)
            {
                throw new FileNotFoundException(path);
            }

            return reader.ReadBytes((int)size);
        }
    }
}