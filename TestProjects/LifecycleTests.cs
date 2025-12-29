// <copyright file="LifecycleTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using Attributes;

namespace TestProjects
{
    /// <summary>
    /// Tests correct execution order of lifecycle methods.
    /// </summary>
    public class LifecycleTests
    {
        private static int _beforeClassCounter;
        private int _value;

        [BeforeClass]
        private static void BeforeAll()
        {
            _beforeClassCounter++;
        }

        [AfterClass]
        private static void AfterAll()
        {
            Console.WriteLine($"BeforeClass executed {_beforeClassCounter} time(s)");
        }

        [Before]
        private void SetUp()
        {
            _value = 42;
        }

        [After]
        private void TearDown()
        {
            _value = 0;
        }

        [Test]
        public void ValueIsInitializedBeforeEachTest()
        {
            if (_value != 42)
            {
                throw new Exception("Before method was not executed");
            }
        }

        [Test]
        public void BeforeClassExecutedOnce()
        {
            if (_beforeClassCounter != 1)
            {
                throw new Exception("BeforeClass should be executed exactly once");
            }
        }
    }
}