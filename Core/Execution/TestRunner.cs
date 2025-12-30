// <copyright file="TestRunner.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Execution
{
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
            var tasks = new List<Task<IReadOnlyCollection<TestResult>>>();

            foreach (var testClass in testClasses)
            {
                tasks.Add(Task.Run(() => RunTestClass(testClass)));
            }

            Task.WaitAll(tasks.ToArray());

            return tasks
                .SelectMany(t => t.Result)
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

                foreach (var test in testClass.Tests)
                {
                    results.Add(RunSingleTest(testClass, test));
                }
            }
            catch (Exception ex)
            {
                foreach (var test in testClass.Tests)
                {
                    results.Add(new TestResult(
                        $"{testClass.ClassType.FullName}.{test.Method.Name}",
                        TestStatus.Failed,
                        0,
                        $"BeforeClass failed: {ex.Message}"));
                }
            }
            finally
            {
                try
                {
                    InvokeStaticMethods(testClass.AfterClassMethods);
                }
                catch
                {
                }
            }

            return results;
        }

        /// <summary>
        /// Executes a single test method with full lifecycle handling.
        /// </summary>
        private TestResult RunSingleTest(TestClassInfo testClass, TestMethodInfo test)
        {
            var testName = $"{testClass.ClassType.FullName}.{test.Method.Name}";

            if (test.Attribute.Ignore != null)
            {
                return new TestResult(
                    testName,
                    TestStatus.Ignored,
                    0,
                    test.Attribute.Ignore);
            }

            var stopwatch = Stopwatch.StartNew();
            object? instance = null;

            try
            {
                instance = Activator.CreateInstance(testClass.ClassType);

                InvokeInstanceMethods(instance, testClass.BeforeMethods);

                test.Method.Invoke(instance, null);

                if (test.Attribute.Expected != null)
                {
                    return new TestResult(
                        testName,
                        TestStatus.Failed,
                        stopwatch.ElapsedMilliseconds,
                        $"Expected exception {test.Attribute.Expected.Name} was not thrown");
                }

                return new TestResult(
                    testName,
                    TestStatus.Passed,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (TargetInvocationException ex)
            {
                var actualException = ex.InnerException!;

                if (test.Attribute.Expected != null &&
                    test.Attribute.Expected.IsAssignableFrom(actualException.GetType()))
                {
                    return new TestResult(
                        testName,
                        TestStatus.Passed,
                        stopwatch.ElapsedMilliseconds);
                }

                return new TestResult(
                    testName,
                    TestStatus.Failed,
                    stopwatch.ElapsedMilliseconds,
                    actualException.ToString());
            }
            catch (Exception ex)
            {
                return new TestResult(
                    testName,
                    TestStatus.Failed,
                    stopwatch.ElapsedMilliseconds,
                    ex.ToString());
            }
            finally
            {
                stopwatch.Stop();

                if (instance != null)
                {
                    try
                    {
                        InvokeInstanceMethods(instance, testClass.AfterMethods);
                    }
                    catch
                    {
                    }
                }
            }
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
}