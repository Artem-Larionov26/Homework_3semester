// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Net;
using SimpleFTP;

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var address = IPAddress.Loopback;
var port = args.Length > 0 && int.TryParse(args[0], out var p) ? p : 5000;

using var server = new Server(address, port);

Console.WriteLine($"Server started on {address}:{port}");
Console.WriteLine("Press Ctrl+C to stop.");

await server.StartAsync(cts.Token);

Console.WriteLine("Server stopped.");