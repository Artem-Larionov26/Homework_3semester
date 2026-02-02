// <copyright file="ListTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using SimpleFTP;
using System.IO;
using System.Linq;

namespace SimpleFTP.Tests;

[TestFixture]
public class ListTests
{
    private string tempDir;
    private Client client;

    [SetUp]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        int port = TestServerHelper.GetFreePort();
        TestServerHelper.StartServer(port);

        client = new Client("127.0.0.1", port);
    }

    [TearDown]
    public void Cleanup() => Directory.Delete(tempDir, true);

    [Test]
    public void List_ExistingDirectory_ReturnsEntries()
    {
        var entries = client.GetList(tempDir);
        Assert.That(entries, Is.Not.Null);
    }

    [Test]
    public void List_NonExistingDirectory_ThrowsException()
    {
        Assert.Throws<DirectoryNotFoundException>(() =>
        {
            client.GetList(Path.Combine(tempDir, "no_such_dir"));
        });
    }

    [Test]
    public void List_ContainsKnownFile()
    {
        string filePath = Path.Combine(tempDir, "test.txt");
        File.WriteAllText(filePath, "hello");

        var entries = client.GetList(tempDir);

        Assert.That(entries.Any(e => e.Name == "test.txt" && !e.IsDirectory));
    }
}