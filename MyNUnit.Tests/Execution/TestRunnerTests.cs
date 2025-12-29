// <copyright file="TestRunnerTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Linq;
using Core.Execution;
using Core.Models;
using Attributes;
using NUnit.Framework;

namespace MyNUnit.Tests.Execution
{
    /// <summary>
    /// Tests for test execution logic.
    /// </summary>
    public class TestRunnerTests
    {
        private class PassingTestClass
        {
            [Test]
            public void PassingTest()
            {
            }
        }

        private class FailingTestClass
        {
            [Test]
            public void FailingTest()
            {
                throw new System.Exception();
            }
        }

        private class ExpectedExceptionTestClass
        {
            [Test(Expected = typeof(System.InvalidOperationException))]
            public void ExpectedExceptionTest()
            {
                throw new System.InvalidOperationException();
            }
        }

        private class IgnoredTestClass
        {
            [Test(Ignore = "Ignored for testing")]
            public void IgnoredTest()
            {
                throw new System.Exception();
            }
        }

        [Test]
        public void Run_PassingTest_ReturnsPassed()
        {
            var testClass = CreateTestClassInfo(typeof(PassingTestClass));
            var runner = new TestRunner();

            var result = runner.Run(new[] { testClass }).Single();

            Assert.AreEqual(TestStatus.Passed, result.Status);
        }

        [Test]
        public void Run_FailingTest_ReturnsFailed()
        {
            var testClass = CreateTestClassInfo(typeof(FailingTestClass));
            var runner = new TestRunner();

            var result = runner.Run(new[] { testClass }).Single();

            Assert.AreEqual(TestStatus.Failed, result.Status);
        }

        [Test]
        public void Run_ExpectedException_ReturnsPassed()
        {
            var testClass = CreateTestClassInfo(typeof(ExpectedExceptionTestClass));
            var runner = new TestRunner();

            var result = runner.Run(new[] { testClass }).Single();

            Assert.AreEqual(TestStatus.Passed, result.Status);
        }

        [Test]
        public void Run_IgnoredTest_ReturnsIgnored()
        {
            var testClass = CreateTestClassInfo(typeof(IgnoredTestClass));
            var runner = new TestRunner();

            var result = runner.Run(new[] { testClass }).Single();

            Assert.AreEqual(TestStatus.Ignored, result.Status);
        }

        private static TestClassInfo CreateTestClassInfo(System.Type type)
        {
            var discoverer = new Core.Discovery.TestDiscoverer();
            return discoverer
                .Discover(System.IO.Path.GetDirectoryName(type.Assembly.Location)!)
                .Single(tc => tc.ClassType == type);
        }
    }
}