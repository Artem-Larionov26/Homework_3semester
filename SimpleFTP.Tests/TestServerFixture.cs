// <copyright file="TestServerFixture.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using SimpleFTP;
using System.Threading;

namespace SimpleFTP.Tests
{
    [SetUpFixture]
    public class TestServerFixture
    {
        public static int Port = 5055;

        [OneTimeSetUp]
        public void StartServer()
        {
            var server = new Server(Port);
            server.Start();

            Thread.Sleep(500);
        }
    }
}