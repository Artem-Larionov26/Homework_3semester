// <copyright file="Server.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleFTP;

/// <summary>
/// Asynchronous FTP-like server supporting LIST and GET commands.
/// </summary>
public sealed class Server(IPAddress address, int port) : IDisposable
{
    private readonly TcpListener listener = new(address, port);
    private readonly CancellationTokenSource cts = new();

    /// <summary>
    /// Starts the server asynchronously.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        listener.Start();

        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        var clientTasks = new List<Task>();

        try
        {
            while (!linkedCts.Token.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(linkedCts.Token);

                var task = HandleClientAsync(client, linkedCts.Token);
                clientTasks.Add(task);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }

        await Task.WhenAll(clientTasks);
    }

    private static async Task HandleClientAsync(
        TcpClient client,
        CancellationToken cancellationToken)
    {
        using var _ = client;
        await using var stream = client.GetStream();

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        string? request = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(request))
        {
            return;
        }

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
                ProcessList(path, writer);
                break;

            case "2":
                await ProcessGetAsync(path, writer, stream, cancellationToken);
                break;

            default:
                writer.Write(-1L);
                break;
        }
    }

    private static void ProcessList(string path, BinaryWriter writer)
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
            writer.Write(Path.GetFileName(entry));
            writer.Write(Directory.Exists(entry));
        }
    }

    private static async Task ProcessGetAsync(
        string path,
        BinaryWriter writer,
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            writer.Write(-1L);
            return;
        }

        var fileInfo = new FileInfo(path);
        writer.Write(fileInfo.Length);

        await using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        await file.CopyToAsync(stream, cancellationToken);
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Dispose()
    {
        cts.Cancel();
        listener.Stop();
        cts.Dispose();
    }
}