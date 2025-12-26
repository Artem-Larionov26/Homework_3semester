// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using DirectoryChecksum.Core;
using DirectoryChecksum.Utilities;
using System.Diagnostics;

namespace DirectoryChecksum
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Directory Checksum Calculator ===\n");

            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            var directoryPath = args[0];

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Error: Directory '{directoryPath}' does not exist!");
                ShowUsage();
                return;
            }

            try
            {
                await RunChecksumCalculation(directoryPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  DirectoryChecksum <path_to_directory>");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  DirectoryChecksum C:\\MyProject");
            Console.WriteLine("  DirectoryChecksum /home/user/documents");
            Console.WriteLine("  DirectoryChecksum .  (current directory)");
        }

        static async Task RunChecksumCalculation(string directoryPath)
        {
            var hasher = new DirectoryHasher();
            var stopwatch = new Stopwatch();

            Console.WriteLine($"Computing checksum for: {Path.GetFullPath(directoryPath)}");
            Console.WriteLine();

            Console.WriteLine("1. Starting single-threaded calculation...");
            stopwatch.Start();
            var singleThreadedHash = hasher.CalculateDirectoryHashSingleThreaded(directoryPath);
            stopwatch.Stop();
            var singleThreadedTime = stopwatch.Elapsed;

            Console.WriteLine($"   Single-threaded hash: {hasher.HashToHexString(singleThreadedHash)}");
            Console.WriteLine($"   Execution time: {singleThreadedTime:hh\\:mm\\:ss\\.fff}");
            Console.WriteLine($"   Total milliseconds: {singleThreadedTime.TotalMilliseconds:F2} ms");

            Console.WriteLine("\n2. Starting multi-threaded calculation...");
            stopwatch.Restart();
            var multiThreadedHash = hasher.CalculateDirectoryHashMultiThreaded(directoryPath);
            stopwatch.Stop();
            var multiThreadedTime = stopwatch.Elapsed;

            Console.WriteLine($"   Multi-threaded hash: {hasher.HashToHexString(multiThreadedHash)}");
            Console.WriteLine($"   Execution time: {multiThreadedTime:hh\\:mm\\:ss\\.fff}");
            Console.WriteLine($"   Total milliseconds: {multiThreadedTime.TotalMilliseconds:F2} ms");

            Console.WriteLine("\n3. Starting async calculation...");
            stopwatch.Restart();
            var asyncHash = await hasher.CalculateDirectoryHashAsync(directoryPath);
            stopwatch.Stop();
            var asyncTime = stopwatch.Elapsed;

            Console.WriteLine($"   Async hash: {hasher.HashToHexString(asyncHash)}");
            Console.WriteLine($"   Execution time: {asyncTime:hh\\:mm\\:ss\\.fff}");
            Console.WriteLine($"   Total milliseconds: {asyncTime.TotalMilliseconds:F2} ms");

            Console.WriteLine("\n4. Starting hybrid async calculation...");
            stopwatch.Restart();
            var hybridHash = await hasher.CalculateDirectoryHashHybridAsync(directoryPath);
            stopwatch.Stop();
            var hybridTime = stopwatch.Elapsed;

            Console.WriteLine($"   Hybrid async hash: {hasher.HashToHexString(hybridHash)}");
            Console.WriteLine($"   Execution time: {hybridTime:hh\\:mm\\:ss\\.fff}");
            Console.WriteLine($"   Total milliseconds: {hybridTime.TotalMilliseconds:F2} ms");

            Console.WriteLine("\n=== Comparison Results ===");
            bool allHashesMatch = hasher.CompareHashes(singleThreadedHash, multiThreadedHash) &&
                                 hasher.CompareHashes(singleThreadedHash, asyncHash) &&
                                 hasher.CompareHashes(singleThreadedHash, hybridHash);

            Console.WriteLine($"All hashes match: {allHashesMatch}");

            if (allHashesMatch)
            {
                Console.WriteLine("\nPerformance comparison (relative to single-threaded):");
                Console.WriteLine($"Single-threaded: 1.00x (baseline)");

                if (singleThreadedTime.TotalMilliseconds > 0)
                {
                    Console.WriteLine($"Multi-threaded:  {singleThreadedTime.TotalMilliseconds / multiThreadedTime.TotalMilliseconds:F2}x");
                    Console.WriteLine($"Async:           {singleThreadedTime.TotalMilliseconds / asyncTime.TotalMilliseconds:F2}x");
                    Console.WriteLine($"Hybrid async:    {singleThreadedTime.TotalMilliseconds / hybridTime.TotalMilliseconds:F2}x");
                }

                var times = new[]
                {
                    ("Single-threaded", singleThreadedTime),
                    ("Multi-threaded", multiThreadedTime),
                    ("Async", asyncTime),
                    ("Hybrid async", hybridTime)
                };

                var fastest = times.OrderBy(t => t.Item2).First();
                Console.WriteLine($"\nFastest method: {fastest.Item1}");
            }
            else
            {
                Console.WriteLine("\nWARNING: Hashes do not match! There might be a bug in the implementation.");
            }

            ShowDirectoryStats(directoryPath);
        }

        static void ShowDirectoryStats(string directoryPath)
        {
            try
            {
                var allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                var allDirectories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);

                Console.WriteLine();
                Console.WriteLine("Directory statistics:");
                Console.WriteLine($"   Files: {allFiles.Length}");
                Console.WriteLine($"   Subdirectories: {allDirectories.Length}");
                Console.WriteLine($"   Total size: {GetTotalSize(allFiles)} bytes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Warning: Could not collect statistics: {ex.Message}");
            }
        }

        static string GetTotalSize(string[] files)
        {
            long totalSize = 0;
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                }
                catch
                {
                }
            }
            return totalSize.ToString("N0");
        }
    }
}