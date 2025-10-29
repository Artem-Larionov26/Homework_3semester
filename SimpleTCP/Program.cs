// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using SimpleFTP;


int port = 5000;

var server = new Server(port);
server.Start();

Thread.Sleep(1000);

var client = new Client("127.0.0.1", port);
client.SendCommand("1 ./");
client.SendCommand("2 ./test.txt");

Console.WriteLine("Press Enter to finish...");
Console.ReadLine();