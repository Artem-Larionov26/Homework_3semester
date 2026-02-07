// <copyright file="TestRunner.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Execution;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Models;

/// <summary>
/// Executes discovered tests and handles their full lifecycle.
/// Test classes are executed in parallel, while tests inside a class
/// are executed sequentially to avoid race conditions.
/// </summary>
public class TestRunner
{
    /// <summary>
    /// Executes all tests in the given test classes.
    /// Test classes are executed in parallel.
    /// </summary>
    public IReadOnlyList<TestResult> Run(IEnumerable<TestClassInfo> testClasses)
    {
        var tasks = testClasses.Select(tc =>
            Task.Run(() => this.RunTestClass(tc)));

        return Task.WhenAll(tasks)
            .Result
            .SelectMany(r => r)
            .ToList();
    }

    /// <summary>
    /// Executes all tests inside a single test class.
    /// Tests inside a class are executed sequentially.
    /// </summary>
    private IReadOnlyCollection<TestResult> RunTestClass(TestClassInfo testClass)
    {
        var results = new List<TestResult>();

        try
        {
            InvokeStaticMethods(testClass.BeforeClassMethods);
        }
        catch (Exception ex)
        {
            foreach (var test in testClass.Tests)
            {
                results.Add(new TestResult(
                    $"{testClass.ClassType.FullName}.{test.Method.Name}",
                    TestStatus.Errored,
                    0,
                    $"BeforeClass failed: {ex}"));
            }

            return results;
        }

        foreach (var test in testClass.Tests)
        {
            results.Add(this.RunSingleTest(testClass, test));
        }

        try
        {
            InvokeStaticMethods(testClass.AfterClassMethods);
        }
        catch (Exception ex)
        {
            for (var i = 0; i < results.Count; i++)
            {
                results[i] = results[i] with
                {
                    Status = TestStatus.Errored,
                    Message = $"AfterClass failed: {ex}",
                };
            }
        }

        return results;
    }

    /// <summary>
    /// Executes a single test method with full lifecycle handling.
    /// </summary>
    private TestResult RunSingleTest(
    TestClassInfo testClass,
    TestMethodInfo test)
    {
        var testName =
            $"{testClass.ClassType.FullName}.{test.Method.Name}";

        if (test.Attribute.Ignore != null)
        {
            return new TestResult(
                testName,
                TestStatus.Ignored,
                0,
                test.Attribute.Ignore);
        }

        var stopwatch = Stopwatch.StartNew();
        object instance;

        try
        {
            instance = Activator.CreateInstance(testClass.ClassType)
                ?? throw new InvalidOperationException(
                    "Test class instance is null");
        }
        catch (Exception ex)
        {
            return new TestResult(
                testName,
                TestStatus.Errored,
                0,
                $"Constructor failed: {ex}");
        }

        try
        {
            InvokeInstanceMethods(instance, testClass.BeforeMethods);
        }
        catch (Exception ex)
        {
            return new TestResult(
                testName,
                TestStatus.Errored,
                stopwatch.ElapsedMilliseconds,
                $"Before failed: {ex}");
        }

        TestResult result;

        try
        {
            test.Method.Invoke(instance, null);

            stopwatch.Stop();

            if (test.Attribute.Expected != null)
            {
                result = new TestResult(
                    testName,
                    TestStatus.Failed,
                    stopwatch.ElapsedMilliseconds,
                    $"Expected exception {test.Attribute.Expected.Name} was not thrown");
            }
            else
            {
                result = new TestResult(
                    testName,
                    TestStatus.Passed,
                    stopwatch.ElapsedMilliseconds);
            }
        }
        catch (TargetInvocationException ex)
        {
            stopwatch.Stop();

            var actual = ex.InnerException ?? ex;

            if (test.Attribute.Expected != null &&
                test.Attribute.Expected.IsAssignableFrom(actual.GetType()))
            {
                result = new TestResult(
                    testName,
                    TestStatus.Passed,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                result = new TestResult(
                    testName,
                    TestStatus.Failed,
                    stopwatch.ElapsedMilliseconds,
                    actual.ToString());
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            result = new TestResult(
                testName,
                TestStatus.Errored,
                stopwatch.ElapsedMilliseconds,
                ex.ToString());
        }

        try
        {
            InvokeInstanceMethods(instance, testClass.AfterMethods);
        }
        catch (Exception ex)
        {
            result = result with
            {
                Status = TestStatus.Errored,
                Message = $"After failed: {ex}",
            };
        }

        return result;
    }

    /// <summary>
    /// Invokes instance lifecycle methods (Before / After).
    /// </summary>
    private static void InvokeInstanceMethods(
        object instance,
        IEnumerable<MethodInfo> methods)
    {
        foreach (var method in methods)
        {
            method.Invoke(instance, null);
        }
    }

    /// <summary>
    /// Invokes static lifecycle methods (BeforeClass / AfterClass).
    /// </summary>
    private static void InvokeStaticMethods(IEnumerable<MethodInfo> methods)
    {
        foreach (var method in methods)
        {
            method.Invoke(null, null);
        }
    }
}