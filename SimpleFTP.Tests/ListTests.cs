// <copyright file="ListTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using SimpleFTP;
using System.IO;
using System.Linq;

namespace SimpleFTP.Tests
{
    [TestFixture]
    public class ListTests
    {
        private Client client;

        [SetUp]
        public void Setup()
        {
            client = new Client("127.0.0.1", TestServerFixture.Port);
        }

        [Test]
        public void List_ExistingDirectory_ReturnsEntries()
        {
            var entries = client.GetList("./");

            Assert.That(entries, Is.Not.Null);
            Assert.That(entries.Length, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void List_NonExistingDirectory_ThrowsException()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                client.GetList("./this_directory_does_not_exist");
            });
        }

        [Test]
        public void List_ContainsKnownFile()
        {
            var testFile = "list_test.txt";
            File.WriteAllText(testFile, "hello");

            var entries = client.GetList("./");

            Assert.That(entries.Any(e => e.Name == testFile && !e.IsDirectory));

            File.Delete(testFile);
        }
    }
}