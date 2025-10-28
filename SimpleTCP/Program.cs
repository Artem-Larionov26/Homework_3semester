// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace MyNetworkingProject
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 5000;

            var server = new Server(port);
            server.Start();

            Thread.Sleep(1000);

            var client = new Client("127.0.0.1", port);
            client.SendMessage("Hello, server!");

            Console.WriteLine("Press Enter to finish...");
            Console.ReadLine();
        }
    }
}