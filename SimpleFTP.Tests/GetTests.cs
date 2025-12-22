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
        public void Cleanup()
        {
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void Get_ExistingFile_ReturnsCorrectContent()
        {
            string filePath = Path.Combine(tempDir, "file.txt");
            string content = "Hello FTP!";
            File.WriteAllText(filePath, content);

            byte[] data = client.GetFile(filePath);
            string received = Encoding.UTF8.GetString(data);

            Assert.That(received, Is.EqualTo(content));
        }

        [Test]
        public void Get_NonExistingFile_ThrowsException()
        {
            string filePath = Path.Combine(tempDir, "no_file.txt");

            Assert.Throws<FileNotFoundException>(() =>
            {
                client.GetFile(filePath);
            });
        }
    }
}