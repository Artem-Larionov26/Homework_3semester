// <copyright file="TestServerHelper.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleFTP.Tests
{
    public static class TestServerHelper
    {
        public static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static Server StartServer(int port)
        {
            var server = new Server(port);
            server.Start();
            Thread.Sleep(200);
            return server;
        }
    }
}
