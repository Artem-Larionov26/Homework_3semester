// <copyright file="TestRunnerTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnit.Tests.Execution;

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
            throw new System.Exception("Failure");
        }
    }

    private class ExpectedExceptionTestClass
    {
        [TestAttribute(Expected = typeof(System.InvalidOperationException))]
        public void ExpectedExceptionTest()
        {
            throw new System.InvalidOperationException();
        }
    }

    private class IgnoredTestClass
    {
        [TestAttribute(Ignore = "Ignored for testing")]
        public void IgnoredTest()
        {
            throw new System.Exception("Should not be executed");
        }
    }

    [Test]
    public void Run_PassingTest_ReturnsPassed()
    {
        var testClass = CreateTestClassInfo(typeof(PassingTestClass));
        var runner = new TestRunner();

        var result = runner.Run(new[] { testClass }).Single();

        Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
    }

    [Test]
    public void Run_FailingTest_ReturnsFailed()
    {
        var testClass = CreateTestClassInfo(typeof(FailingTestClass));
        var runner = new TestRunner();

        var result = runner.Run(new[] { testClass }).Single();

        Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
    }

    [Test]
    public void Run_ExpectedException_ReturnsPassed()
    {
        var testClass = CreateTestClassInfo(typeof(ExpectedExceptionTestClass));
        var runner = new TestRunner();

        var result = runner.Run(new[] { testClass }).Single();

        Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
    }

    [Test]
    public void Run_IgnoredTest_ReturnsIgnored()
    {
        var testClass = CreateTestClassInfo(typeof(IgnoredTestClass));
        var runner = new TestRunner();

        var result = runner.Run(new[] { testClass }).Single();

        Assert.That(result.Status, Is.EqualTo(TestStatus.Ignored));
    }

    /// <summary>
    /// Helper method that discovers TestClassInfo for a given type
    /// using the real TestDiscoverer.
    /// </summary>
    private static TestClassInfo CreateTestClassInfo(System.Type type)
    {
        var discoverer = new TestDiscoverer();

        return discoverer
            .Discover(System.IO.Path.GetDirectoryName(type.Assembly.Location)!)
            .Single(tc => tc.ClassType == type);
    }
}