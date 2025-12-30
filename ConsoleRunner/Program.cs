// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using Core.Discovery;
using Core.Execution;
using Core.Models;

namespace ConsoleRunner
{
    /// <summary>
    /// Entry point of the MyNUnit console runner.
    /// Responsible for parsing arguments, executing tests
    /// and printing a human-readable execution report.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MyNUnit <path-to-directory-with-assemblies>");
                return 1;
            }

            var path = args[0];

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory not found: {path}");
                return 1;
            }

            try
            {
                var discoverer = new TestDiscoverer();
                var testClasses = discoverer.Discover(path);

                var runner = new TestRunner();
                var results = runner.Run(testClasses);

                PrintReport(results);

                return results.Any(r => r.Status == TestStatus.Failed) ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error occurred during test execution:");
                Console.WriteLine(ex);
                return 1;
            }
        }

        /// <summary>
        /// Prints a formatted test execution report to standard output.
        /// </summary>
        private static void PrintReport(System.Collections.Generic.IReadOnlyCollection<TestResult> results)
        {
            int passed = 0;
            int failed = 0;
            int ignored = 0;

            foreach (var result in results)
            {
                switch (result.Status)
                {
                    case TestStatus.Passed:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(
                            $"[PASS] {result.TestName} ({result.ExecutionTimeMs} ms)");
                        passed++;
                        break;

                    case TestStatus.Failed:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            $"[FAIL] {result.TestName} ({result.ExecutionTimeMs} ms)");
                        Console.ResetColor();
                        Console.WriteLine(result.Message);
                        failed++;
                        break;

                    case TestStatus.Ignored:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(
                            $"[IGNORED] {result.TestName} — {result.Message}");
                        ignored++;
                        break;
                }

                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Summary:");
            Console.WriteLine($"  Passed:  {passed}");
            Console.WriteLine($"  Failed:  {failed}");
            Console.WriteLine($"  Ignored: {ignored}");
            Console.WriteLine($"  Total:   {passed + failed + ignored}");
        }
    }
}