// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using Core.Discovery;
using Core.Execution;

namespace ConsoleRunner
{
    /// <summary>
    /// Entry point of the MyNUnit console runner.
    /// Responsible for parsing arguments and printing execution results.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MyNUnit <path-to-directory-with-assemblies>");
                return;
            }

            var path = args[0];

            if (!System.IO.Directory.Exists(path))
            {
                Console.WriteLine($"Directory not found: {path}");
                return;
            }

            try
            {
                var discoverer = new TestDiscoverer();
                var testClasses = discoverer.Discover(path);

                var runner = new TestRunner();
                var results = runner.Run(testClasses);

                PrintReport(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error occurred during test execution:");
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Prints a formatted test execution report to standard output.
        /// </summary>
        private static void PrintReport(System.Collections.Generic.IEnumerable<TestResult> results)
        {
            foreach (var result in results)
            {
                switch (result.Status)
                {
                    case TestStatus.Passed:
                        Console.WriteLine(
                            $"[PASS] {result.TestName} ({result.ExecutionTimeMs} ms)");
                        break;

                    case TestStatus.Failed:
                        Console.WriteLine(
                            $"[FAIL] {result.TestName} ({result.ExecutionTimeMs} ms)");
                        Console.WriteLine(result.Message);
                        break;

                    case TestStatus.Ignored:
                        Console.WriteLine(
                            $"[IGNORED] {result.TestName} — {result.Message}");
                        break;
                }
            }
        }
    }
}