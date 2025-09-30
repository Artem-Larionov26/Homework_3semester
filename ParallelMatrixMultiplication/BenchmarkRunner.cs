// <copyright file="BenchmarkRunner.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace ParallelMatrixMultiplication
{
    /// <summary>
    /// Runs benchmarks for different matrix sizes and prints results in table format.
    /// </summary>
    public static class BenchmarkRunner
    {
        /// <summary>
        /// Runs benchmarks for predefined sizes and prints a formatted table.
        /// </summary>
        public static void Run()
        {
            int[] sizes = { 100, 200, 500 }; // sizes for testing
            int runs = 5; // number of runs per case

            // Заголовок таблицы
            Console.WriteLine("\nBenchmark results:");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"{"Size",8} | {"Seq Mean (ms)",15} | {"Seq Std",10} | {"Par Mean (ms)",15} | {"Par Std",10} | {"Speedup",8}");
            Console.WriteLine("------------------------------------------------------------");

            foreach (int size in sizes)
            {
                int[,] A = MatrixUserInterface.GenerateRandomMatrixInMemory(size, size, -10, 10);
                int[,] B = MatrixUserInterface.GenerateRandomMatrixInMemory(size, size, -10, 10);

                var (meanSeq, stdSeq) = Benchmark.Run(A, B, MatrixMultiplication.MultiplySequential, runs);
                var (meanPar, stdPar) = Benchmark.Run(A, B, MatrixMultiplication.MultiplyParallelThreadLimited, runs);

                double speedup = meanSeq / meanPar;

                Console.WriteLine($"{size,8} | {meanSeq,15:F2} | {stdSeq,10:F2} | {meanPar,15:F2} | {stdPar,10:F2} | {speedup,8:F2}");
            }

            Console.WriteLine("------------------------------------------------------------\n");
        }
    }
}