// <copyright file="GetTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using SimpleFTP;
using System.IO;
using System.Text;

namespace SimpleFTP.Tests
{
    [TestFixture]
    public class GetTests
    {
        private Client client;

        [SetUp]
        public void Setup()
        {
            client = new Client("127.0.0.1", TestServerFixture.Port);
        }

        [Test]
        public void Get_ExistingFile_ReturnsCorrectContent()
        {
            var path = "get_test.txt";
            var content = "Hello FTP!";
            File.WriteAllText(path, content);

            byte[] data = client.GetFile(path);
            string received = Encoding.UTF8.GetString(data);

            Assert.That(received, Is.EqualTo(content));

            File.Delete(path);
        }

        [Test]
        public void Get_NonExistingFile_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                client.GetFile("no_such_file.txt");
            });
        }
    }
}