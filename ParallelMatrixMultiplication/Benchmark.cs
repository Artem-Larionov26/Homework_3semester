// <copyright file="Benchmark.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace ParallelMatrixMultiplication
{
    /// <summary>
    /// Provides benchmarking utilities for comparing
    /// sequential and parallel matrix multiplication algorithms.
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Runs the given multiplication function multiple times
        /// and calculates mean execution time and standard deviation.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <param name="B">Matrix B.</param>
        /// <param name="multiplyFunc">
        /// A function that takes two matrices and returns their product.
        /// For example: MatrixMultiplication.MultiplySequential or MatrixMultiplication.MultiplyParallelThreadLimited.
        /// </param>
        /// <param name="runs">Number of times to repeat the benchmark (e.g., 10 or 20).</param>
        public static (double mean, double std) Run(
            int[,] A, int[,] B,
            Func<int[,], int[,], int[,]> multiply,
            int runs)
        {
            var times = new List<double>();
            var sw = new Stopwatch();

            for (int i = 0; i < runs; i++)
            {
                sw.Restart();
                multiply(A, B);
                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
            }

            double mean = times.Average();
            double variance = times.Sum(t => (t - mean) * (t - mean)) / times.Count;
            double std = Math.Sqrt(variance);

            return (mean, std);
        }
    }
}