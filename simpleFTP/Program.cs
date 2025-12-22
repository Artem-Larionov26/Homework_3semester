// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Threading;
using SimpleFTP;

int port = 5000;

var server = new Server(port);
server.Start();

Thread.Sleep(500);

var client = new Client("127.0.0.1", port);

client.SendList("./");
client.SendGet("./test.txt");

Console.WriteLine("Press Enter to exit...");
Console.ReadLine();