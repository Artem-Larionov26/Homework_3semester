using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Tests
{
    [TestFixture]
    public class ChatSessionTests
    {
        [Test]
        public void ChatSession_CanBeCreated()
        {
            using var client = new TcpClient();
            var input = new StringReader("");
            var output = new StringWriter();

            var session = new ChatSession(client, input, output);

            Assert.That(session, Is.Not.Null);
        }

        [Test]
        public async Task ChatSession_ExitCommand_DoesNotThrow()
        {
            var input = new StringReader("exit\n");
            var output = new StringWriter();

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var clientTask = Task.Run(async () =>
            {
                using var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, port);
                var session = new ChatSession(client, input, output);
                await session.RunAsync();
            });

            using var serverClient = await listener.AcceptTcpClientAsync();
            using var stream = serverClient.GetStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            await writer.WriteLineAsync("exit");

            await clientTask;
            listener.Stop();

            Assert.Pass();
        }

        [Test]
        public void IsExit_RecognizesExitCommand()
        {
            Assert.That(
                typeof(ChatSession)
                    .GetMethod("IsExit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    ?.Invoke(null, new object[] { "EXIT" }),
                Is.EqualTo(true)
            );
        }
    }
}
