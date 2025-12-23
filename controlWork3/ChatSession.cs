using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp
{
    /// <summary>
    /// Represents a chat session between two connected peers.
    /// Handles sending and receiving messages asynchronously.
    /// </summary>
    public class ChatSession
    {
        private readonly TcpClient client;
        private readonly TextReader input;
        private readonly TextWriter output;

        public ChatSession(TcpClient client, TextReader input, TextWriter output)
        {
            this.client = client;
            this.input = input;
            this.output = output;
        }

        public async Task RunAsync()
        {
            using var cts = new CancellationTokenSource();

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            var receiveTask = ReceiveLoopAsync(reader, cts.Token);
            var sendTask = SendLoopAsync(writer, cts);

            await Task.WhenAny(receiveTask, sendTask);

            cts.Cancel();
            client.Close();
        }

        private async Task ReceiveLoopAsync(StreamReader reader, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null || IsExit(line))
                    {
                        break;
                    }

                    output.WriteLine($"Remote: {line}");
                }
            }
            catch (IOException)
            {
            }
        }

        private async Task SendLoopAsync(StreamWriter writer, CancellationTokenSource cts)
        {
            try
            {
                while (true)
                {
                    var line = await input.ReadLineAsync();
                    if (line == null)
                    {
                        continue;
                    }

                    await writer.WriteLineAsync(line);

                    if (IsExit(line))
                    {
                        cts.Cancel();
                        break;
                    }
                }
            }
            catch (IOException)
            {
                cts.Cancel();
            }
        }

        private static bool IsExit(string message) =>
            message.Equals("exit", StringComparison.OrdinalIgnoreCase);
    }
}