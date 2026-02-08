// <copyright file="TestRunnerTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnit.Tests.Execution;

using System;
using System.Linq;
using Attributes;
using Core.Discovery;
using Core.Execution;
using Core.Models;
using NUnit.Framework;
using TestAttribute = Attributes.TestAttribute;

/// <summary>
/// Unit tests for TestRunner execution logic.
/// </summary>
public class TestRunnerTests
{
    private class PassingTestClass
    {
        [TestAttribute]
        public void PassingTest()
        {
        }
    }

    private class FailingTestClass
    {
        [TestAttribute]
        public void FailingTest()
        {
            throw new Exception("Failure");
        }
    }

    private class ExpectedExceptionTestClass
    {
        [TestAttribute(Expected = typeof(InvalidOperationException))]
        public void ExpectedExceptionTest()
        {
            throw new InvalidOperationException();
        }
    }

    private class WrongExpectedExceptionTestClass
    {
        [TestAttribute(Expected = typeof(InvalidOperationException))]
        public void WrongExceptionThrown()
        {
            throw new ArgumentException("Wrong exception");
        }
    }

    private class IgnoredTestClass
    {
        [TestAttribute(Ignore = "Ignored for testing")]
        public void IgnoredTest()
        {
            throw new Exception("Should not be executed");
        }
    }

    private class BeforeThrowsTestClass
    {
        [Before]
        public void Before()
        {
            throw new Exception("Before failed");
        }

        [TestAttribute]
        public void TestMethod()
        {
        }
    }

    private class AfterThrowsTestClass
    {
        [After]
        public void After()
        {
            throw new Exception("After failed");
        }

        [TestAttribute]
        public void TestMethod()
        {
        }
    }

    [Test]
    public void Run_PassingTest_ReturnsPassed()
    {
        var result = RunSingleTest(typeof(PassingTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
    }

    [Test]
    public void Run_FailingTest_ReturnsFailed()
    {
        var result = RunSingleTest(typeof(FailingTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
    }

    [Test]
    public void Run_ExpectedException_ReturnsPassed()
    {
        var result = RunSingleTest(typeof(ExpectedExceptionTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
    }

    [Test]
    public void Run_WrongExpectedException_ReturnsFailed()
    {
        var result = RunSingleTest(typeof(WrongExpectedExceptionTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
    }

    [Test]
    public void Run_IgnoredTest_ReturnsIgnored()
    {
        var result = RunSingleTest(typeof(IgnoredTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Ignored));
    }

    [Test]
    public void Run_BeforeThrows_ReturnsErrored()
    {
        var result = RunSingleTest(typeof(BeforeThrowsTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Errored));
    }

    [Test]
    public void Run_AfterThrows_ReturnsErrored()
    {
        var result = RunSingleTest(typeof(AfterThrowsTestClass));
        Assert.That(result.Status, Is.EqualTo(TestStatus.Errored));
    }

    private static TestResult RunSingleTest(Type testClassType)
    {
        var discoverer = new TestDiscoverer();
        var runner = new TestRunner();

        var testClass = discoverer
            .Discover(System.IO.Path.GetDirectoryName(testClassType.Assembly.Location)!)
            .Single(tc => tc.ClassType == testClassType);

        return runner.Run(new[] { testClass }).Single();
    }
}