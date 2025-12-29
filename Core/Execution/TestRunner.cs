// <copyright file="TestRunner.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Core.Models;

namespace Core.Execution
{
    /// <summary>
    /// Executes discovered tests and handles their lifecycle.
    /// </summary>
    public class TestRunner
    {
        /// <summary>
        /// Executes all tests in the given test classes.
        /// </summary>
        public IReadOnlyList<TestResult> Run(IEnumerable<TestClassInfo> testClasses)
        {
            var results = new List<TestResult>();

            foreach (var testClass in testClasses)
            {
                results.AddRange(RunTestClass(testClass));
            }

            return results;
        }

        /// <summary>
        /// Executes all tests inside a single test class.
        /// </summary>
        private IEnumerable<TestResult> RunTestClass(TestClassInfo testClass)
        {
            var results = new List<TestResult>();

            InvokeStaticMethods(testClass.BeforeClassMethods);

            foreach (var test in testClass.Tests)
            {
                results.Add(RunSingleTest(testClass, test));
            }

            InvokeStaticMethods(testClass.AfterClassMethods);

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

            object? instance = null;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                instance = Activator.CreateInstance(testClass.ClassType);

                InvokeInstanceMethods(instance, testClass.BeforeMethods);

                test.Method.Invoke(instance, null);

                InvokeInstanceMethods(instance, testClass.AfterMethods);

                stopwatch.Stop();

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
                stopwatch.Stop();
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
                stopwatch.Stop();

                return new TestResult(
                    testName,
                    TestStatus.Failed,
                    stopwatch.ElapsedMilliseconds,
                    ex.ToString());
            }
        }

        /// <summary>
        /// Invokes instance lifecycle methods (Before / After).
        /// </summary>
        private static void InvokeInstanceMethods(object instance, IEnumerable<MethodInfo> methods)
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