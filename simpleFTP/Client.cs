// <copyright file="Client.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleFTP;

/// <summary>
/// Asynchronous client for SimpleFTP server.
/// </summary>
public class Client(string host, int port)
{
    private readonly string host = host;
    private readonly int port = port;

    /// <summary>
    /// Asynchronously gets directory listing from the server.
    /// </summary>
    public async Task<DirectoryEntry[]> GetListAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        using var client = new TcpClient();

        await client.ConnectAsync(host, port, cancellationToken);
        await using var stream = client.GetStream();

        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true)
        {
            AutoFlush = true
        };
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

        await writer.WriteLineAsync($"1 {path}");

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
    /// Asynchronously downloads file content from the server.
    /// Intended mainly for tests.
    /// </summary>
    public async Task<byte[]> GetFileAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        using var client = new TcpClient();

        await client.ConnectAsync(host, port, cancellationToken);
        await using var stream = client.GetStream();

        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true)
        {
            AutoFlush = true
        };
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

        await writer.WriteLineAsync($"2 {path}");

        long size = reader.ReadInt64();
        if (size == -1)
        {
            throw new FileNotFoundException(path);
        }

        return reader.ReadBytes((int)size);
    }

    public DirectoryEntry[] GetList(string path) =>
        GetListAsync(path).GetAwaiter().GetResult();

    public byte[] GetFile(string path) =>
        GetFileAsync(path).GetAwaiter().GetResult();
}